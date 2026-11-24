using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Common.model.dto;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.service.impl;

namespace Common.controller;

public interface IAttachmentUploadService
{
    Task<List<AttachmentDto>> CreateAsync(IReadOnlyList<IFormFile> files, string entityName, long entityId,
        string description, CancellationToken ct);
}

public sealed class AttachmentUploadValidationException(string message) : Exception(message);

public sealed class AttachmentUploadService(ServiceRuntime runtime, IConfiguration configuration,
    IWebHostEnvironment environment, ILogger<AttachmentUploadService> logger) : IAttachmentUploadService
{
    private static readonly HashSet<string> Extensions = new(StringComparer.OrdinalIgnoreCase)
        { ".jpg", ".pdf", ".svg", ".png", ".jpeg", ".gif", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".json", ".xml", ".rar", ".zip" };

    public async Task<List<AttachmentDto>> CreateAsync(IReadOnlyList<IFormFile> files, string entityName, long entityId,
        string description, CancellationToken ct)
    {
        if (files.Count == 0 || files.Any(file => file.Length <= 0))
            throw new AttachmentUploadValidationException("انتخاب فایل غیرخالی الزامی است.");
        if (files.Any(file => !Extensions.Contains(Path.GetExtension(file.FileName))))
            throw new AttachmentUploadValidationException("فایل بارگذاری شده مجاز نیست.");
        if (description.Length > 200 || entityName.Length > 50 || entityName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            throw new AttachmentUploadValidationException("نام یا توضیح پیوست معتبر نیست.");

        var root = Path.GetFullPath(configuration["Storage:Location"] ?? "Upload", environment.ContentRootPath);
        EnsureSafeDirectory(root);
        var createdFiles = new List<string>();
        var result = new List<AttachmentDto>();
        await runtime.Db.Database.OpenConnectionAsync(ct);
        var connection = runtime.Db.Database.GetDbConnection();
        DbTransaction transaction;
        try { transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct); }
        catch { await runtime.Db.Database.CloseConnectionAsync(); throw; }
        await using var ownedTransaction = transaction;
        try
        {
            var lockKey = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{entityName}\n{entityId}")));
            await using (var lockCommand = Command(connection, transaction,
                "DECLARE @result int; EXEC @result = sys.sp_getapplock @Resource=@resource, @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=15000; SELECT @result;"))
            {
                Parameter(lockCommand, "@resource", "sem-attachment-" + lockKey);
                if (Convert.ToInt32(await lockCommand.ExecuteScalarAsync(ct), CultureInfo.InvariantCulture) < 0)
                    throw new InvalidOperationException("Could not acquire the attachment naming lock.");
            }

            long counter = 0;
            await using (var lastCommand = Command(connection, transaction,
                "SELECT TOP (1) filename FROM dbo.attachment WHERE entityname=@entityName AND entityid=@entityId ORDER BY attachmentid DESC"))
            {
                Parameter(lastCommand, "@entityName", entityName);
                Parameter(lastCommand, "@entityId", entityId);
                if (await lastCommand.ExecuteScalarAsync(ct) is string lastName)
                {
                    var stem = Path.GetFileNameWithoutExtension(lastName);
                    if (!long.TryParse(stem[(stem.LastIndexOf('_') + 1)..], NumberStyles.None, CultureInfo.InvariantCulture, out counter))
                        throw new InvalidOperationException("The latest attachment has an invalid filename counter.");
                }
            }

            var now = TimeZoneInfo.ConvertTime(runtime.Clock.GetUtcNow(), TimeZoneInfo.FindSystemTimeZoneById("Asia/Tehran")).DateTime;
            var calendar = new PersianCalendar();
            var nextYear = calendar.ToDateTime(calendar.GetYear(now) + 1, 1, 1, 0, 0, 0, 0);
            var firstWeekDay = nextYear.AddDays(-((int)nextYear.DayOfWeek - (int)DayOfWeek.Saturday + 7) % 7);
            var weekYear = calendar.GetYear(now) + (now.Date >= firstWeekDay.Date ? 1 : 0);
            var date = $"{weekYear:D4}{calendar.GetMonth(now):D2}{calendar.GetDayOfMonth(now):D2}";
            var userId = runtime.User.Id;
            foreach (var file in files)
            {
                string filename;
                FileStream stream;
                while (true)
                {
                    counter = checked(counter + 1);
                    filename = $"{entityName}_{entityId}_{date}_{counter}{Path.GetExtension(file.FileName)}";
                    if (filename.Length > 91) throw new AttachmentUploadValidationException("نام پیوست بیش از حد طولانی است.");
                    var path = Path.Combine(root, filename);
                    try
                    {
                        stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 65536, FileOptions.Asynchronous);
                        createdFiles.Add(path);
                        break;
                    }
                    catch (IOException) when (File.Exists(path)) { }
                }
                await using (stream) await file.CopyToAsync(stream, ct);

                await using var insert = Command(connection, transaction, """
                    DECLARE @ids TABLE (id bigint);
                    INSERT INTO dbo.attachment (filename, entityname, entityid, uploadurl, des, deleted, createddate, modifieddate, createdby, modifiedby, version)
                    OUTPUT INSERTED.attachmentid INTO @ids
                    VALUES (@filename, @entityName, @entityId, @uploadUrl, @description, 0, @now, @now, @userId, @userId, 0);
                    SELECT id FROM @ids;
                    """);
                Parameter(insert, "@filename", filename);
                Parameter(insert, "@entityName", entityName);
                Parameter(insert, "@entityId", entityId);
                Parameter(insert, "@uploadUrl", "~/Upload/" + filename);
                Parameter(insert, "@description", description);
                Parameter(insert, "@now", now);
                Parameter(insert, "@userId", userId);
                var insertedId = Convert.ToInt64(await insert.ExecuteScalarAsync(ct), CultureInfo.InvariantCulture);
                result.Add(new AttachmentDto { Id = insertedId, FileName = filename, Des = description });
            }
            await transaction.CommitAsync(ct);
            return result;
        }
        catch
        {
            var rolledBack = false;
            try { await transaction.RollbackAsync(CancellationToken.None); rolledBack = true; }
            catch (Exception exception) { logger.LogError(exception, "Attachment transaction outcome is uncertain; uploaded files were retained."); }
            if (rolledBack)
                foreach (var path in createdFiles)
                    try { File.Delete(path); }
                    catch (Exception exception) { logger.LogError(exception, "Could not remove an uncommitted attachment file."); }
            throw;
        }
        finally { await runtime.Db.Database.CloseConnectionAsync(); }
    }

    private static DbCommand Command(DbConnection connection, DbTransaction transaction, string sql)
    {
        var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        return command;
    }

    private static void Parameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    private static void EnsureSafeDirectory(string root)
    {
        var directory = new DirectoryInfo(root);
        for (var current = directory; current is not null; current = current.Parent)
            if (current.Exists && (current.Attributes & FileAttributes.ReparsePoint) != 0)
                throw new AttachmentUploadValidationException("مسیر ذخیره‌سازی پیوست معتبر نیست.");
        Directory.CreateDirectory(root);
        for (var current = directory; current is not null; current = current.Parent)
            if ((current.Attributes & FileAttributes.ReparsePoint) != 0)
                throw new AttachmentUploadValidationException("مسیر ذخیره‌سازی پیوست معتبر نیست.");
    }
}
