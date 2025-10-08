using StockDamage.Web.Models;

namespace StockDamage.Web.Services;

public interface IDatabaseService
{
    Task<IEnumerable<Godown>> GetGodownsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<SubItem>> GetSubItemsAsync(CancellationToken cancellationToken);
    Task<SubItem?> GetSubItemAsync(string subItemCode, CancellationToken cancellationToken);
    Task<decimal?> GetStockForSubItemAsync(string subItemCode, CancellationToken cancellationToken);
    Task<IEnumerable<CurrencyInfo>> GetCurrenciesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Employee>> GetEmployeesAsync(CancellationToken cancellationToken);
    Task SaveStockDamageAsync(StockDamageSaveRequest request, CancellationToken cancellationToken);
}
