using System.Data;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.migration;

public class SupremeBackfillService
{
    private readonly SemDbContext _context;

    public SupremeBackfillService(SemDbContext context) => _context = context;

    // معادل columnExists جاوا؛ ستون AddToSupremLevel در جدول داده‌شده وجود دارد یا نه
    public async Task<bool> ColumnExistsAsync(string table, CancellationToken ct = default)
    {
        var connection = _context.Database.GetDbConnection();
        var wasClosed = connection.State != ConnectionState.Open;
        if (wasClosed) await connection.OpenAsync(ct);
        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*)
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = @table AND COLUMN_NAME = @column";

            var pTable = command.CreateParameter();
            pTable.ParameterName = "@table";
            pTable.Value = table;
            command.Parameters.Add(pTable);

            var pColumn = command.CreateParameter();
            pColumn.ParameterName = "@column";
            pColumn.Value = "AddToSupremLevel";
            command.Parameters.Add(pColumn);

            var count = Convert.ToInt32(await command.ExecuteScalarAsync(ct));
            return count > 0;
        }
        finally
        {
            if (wasClosed) await connection.CloseAsync();
        }
    }
}
