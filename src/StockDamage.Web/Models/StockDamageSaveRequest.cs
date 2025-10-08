namespace StockDamage.Web.Models;

public class StockDamageSaveRequest
{
    public DateTime Date { get; set; }
    public string VoucherNo { get; set; } = string.Empty;
    public string DrAccountHead { get; set; } = "Stock Damage";
    public IList<StockDamageLineItem> Items { get; set; } = new List<StockDamageLineItem>();
}

public class StockDamageLineItem
{
    public int GodownNo { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string SubItemCode { get; set; } = string.Empty;
    public string SubItemName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Stock { get; set; }
    public string BatchNo { get; set; } = "NA";
    public string CurrencyName { get; set; } = string.Empty;
    public decimal CurrencyRate { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal AmountIn { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
}
