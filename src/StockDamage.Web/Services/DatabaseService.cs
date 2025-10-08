using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using StockDamage.Web.Models;

namespace StockDamage.Web.Services;

public class DatabaseService(IConfiguration configuration, ILogger<DatabaseService> logger) : IDatabaseService
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
    private readonly ILogger<DatabaseService> _logger = logger;

    private SqlConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<Godown>> GetGodownsAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT GodownNo, GodownName, AutoSlNo FROM Godown ORDER BY GodownName";
        await using var connection = CreateConnection();
        return await connection.QueryAsync<Godown>(new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<SubItem>> GetSubItemsAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT AutoSlNo, SubItemCode, SubItemName, Unit, Weight FROM SubItemCode ORDER BY SubItemName";
        await using var connection = CreateConnection();
        return await connection.QueryAsync<SubItem>(new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<SubItem?> GetSubItemAsync(string subItemCode, CancellationToken cancellationToken)
    {
        const string sql = "SELECT TOP 1 AutoSlNo, SubItemCode, SubItemName, Unit, Weight FROM SubItemCode WHERE SubItemCode = @Code";
        await using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SubItem>(
            new CommandDefinition(sql, new { Code = subItemCode }, cancellationToken: cancellationToken));
    }

    public async Task<decimal?> GetStockForSubItemAsync(string subItemCode, CancellationToken cancellationToken)
    {
        const string sql = "SELECT TOP 1 Stock FROM Stock WHERE SubItemCode = @Code";
        await using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<decimal?>(
            new CommandDefinition(sql, new { Code = subItemCode }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<CurrencyInfo>> GetCurrenciesAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT CurrencyName, ConversionRate FROM Currency ORDER BY CurrencyName";
        await using var connection = CreateConnection();
        return await connection.QueryAsync<CurrencyInfo>(new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Employee>> GetEmployeesAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT EmployeeId, EmployeeName FROM Employee ORDER BY EmployeeName";
        await using var connection = CreateConnection();
        return await connection.QueryAsync<Employee>(new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task SaveStockDamageAsync(StockDamageSaveRequest request, CancellationToken cancellationToken)
    {
        const string storedProcedure = "SP_StockDamage_Save";

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var item in request.Items)
            {
                var parameters = new
                {
                    request.Date,
                    request.VoucherNo,
                    request.DrAccountHead,
                    item.GodownNo,
                    item.WarehouseName,
                    item.SubItemCode,
                    item.SubItemName,
                    item.Unit,
                    item.Stock,
                    item.BatchNo,
                    item.CurrencyName,
                    item.CurrencyRate,
                    item.Quantity,
                    item.Rate,
                    item.AmountIn,
                    item.EmployeeId,
                    item.Comments
                };

                await connection.ExecuteAsync(
                    storedProcedure,
                    param: parameters,
                    transaction: (SqlTransaction)transaction,
                    commandType: CommandType.StoredProcedure);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to save stock damage entries");
            throw;
        }
    }
}
