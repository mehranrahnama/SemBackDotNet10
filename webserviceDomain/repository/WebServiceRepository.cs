using System.Data;
using System.Globalization;
using SemBackDotNet10.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace SemBackDotNet10.webserviceDomain.repository;

public interface IWebServiceRepository
{
    Task<double> ObjectFunctionalityAsync(long planId, long month, CancellationToken ct = default);

    Task<double> ActionPCNScoreAsync(long planId, long month, CancellationToken ct = default);
}

public class WebServiceRepository : IWebServiceRepository
{
    private readonly SemDbContext _context;

    public WebServiceRepository(SemDbContext context) => _context = context;

    public Task<double> ObjectFunctionalityAsync(long planId, long month, CancellationToken ct = default) =>
        ExecuteScalarProcedureAsync("Report_ObjectFunctionality_Hosco_Score", planId, month, ct);

    public Task<double> ActionPCNScoreAsync(long planId, long month, CancellationToken ct = default) =>
        ExecuteScalarProcedureAsync("Report_Action_PCN_Score", planId, month, ct);

    private async Task<double> ExecuteScalarProcedureAsync(string procedure, long planId, long month, CancellationToken ct)
    {
        var connection = _context.Database.GetDbConnection();
        var wasClosed = connection.State != ConnectionState.Open;
        if (wasClosed)
            await connection.OpenAsync(ct);
        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = $"EXEC {procedure} @p1, @p2";
            command.CommandType = CommandType.Text;
            command.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

            var p1 = command.CreateParameter();
            p1.ParameterName = "@p1";
            p1.Value = planId;
            command.Parameters.Add(p1);

            var p2 = command.CreateParameter();
            p2.ParameterName = "@p2";
            p2.Value = month;
            command.Parameters.Add(p2);

            var result = await command.ExecuteScalarAsync(ct);
            if (result == null || result is DBNull)
                throw new InvalidOperationException($"Stored procedure {procedure} returned no result.");
            return double.Parse(Convert.ToString(result, CultureInfo.InvariantCulture)!, CultureInfo.InvariantCulture);
        }
        finally
        {
            if (wasClosed)
                await connection.CloseAsync();
        }
    }
}
