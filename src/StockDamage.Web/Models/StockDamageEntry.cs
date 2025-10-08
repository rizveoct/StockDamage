namespace StockDamage.Web.Models;

public record StockDamageEntry(
    DateTime Date,
    string VoucherNo,
    int GodownNo,
    string WarehouseName,
    string SubItemCode,
    string SubItemName,
    string Unit,
    decimal Stock,
    string BatchNo,
    string CurrencyName,
    decimal CurrencyRate,
    decimal Quantity,
    decimal Rate,
    decimal AmountIn,
    string DrAccountHead,
    int EmployeeId,
    string EmployeeName,
    string Comments
);
