using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockDamage.Web.Models;
using StockDamage.Web.Services;

namespace StockDamage.Web.Pages;

public class StockDamageModel(IDatabaseService databaseService, ILogger<StockDamageModel> logger) : PageModel
{
    private readonly IDatabaseService _databaseService = databaseService;
    private readonly ILogger<StockDamageModel> _logger = logger;

    [BindProperty]
    public StockDamageSaveRequest Input { get; set; } = new() { Date = DateTime.Today };

    public IEnumerable<Godown> Godowns { get; private set; } = Enumerable.Empty<Godown>();
    public IEnumerable<SubItem> SubItems { get; private set; } = Enumerable.Empty<SubItem>();
    public IEnumerable<CurrencyInfo> Currencies { get; private set; } = Enumerable.Empty<CurrencyInfo>();
    public IEnumerable<Employee> Employees { get; private set; } = Enumerable.Empty<Employee>();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadStaticDataAsync(cancellationToken);
    }

    public async Task<IActionResult> OnGetSubItemAsync(string code, CancellationToken cancellationToken)
    {
        var item = await _databaseService.GetSubItemAsync(code, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        var stock = await _databaseService.GetStockForSubItemAsync(code, cancellationToken) ?? 0m;
        return new JsonResult(new { item.SubItemCode, item.SubItemName, item.Unit, Stock = stock });
    }

    public async Task<IActionResult> OnGetCurrenciesAsync(CancellationToken cancellationToken)
    {
        var currencies = await _databaseService.GetCurrenciesAsync(cancellationToken);
        return new JsonResult(currencies);
    }

    public async Task<IActionResult> OnGetEmployeesAsync(CancellationToken cancellationToken)
    {
        var employees = await _databaseService.GetEmployeesAsync(cancellationToken);
        return new JsonResult(employees);
    }

    public async Task<IActionResult> OnGetGodownsAsync(CancellationToken cancellationToken)
    {
        var godowns = await _databaseService.GetGodownsAsync(cancellationToken);
        return new JsonResult(godowns);
    }

    public async Task<IActionResult> OnPostAsync([FromBody] StockDamageSaveRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (request.Items == null || request.Items.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "At least one entry is required to save stock damage.");
            return BadRequest(ModelState);
        }

        try
        {
            await _databaseService.SaveStockDamageAsync(request, cancellationToken);
            return new JsonResult(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving stock damage entries");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while saving.");
        }
    }

    private async Task LoadStaticDataAsync(CancellationToken cancellationToken)
    {
        Godowns = await _databaseService.GetGodownsAsync(cancellationToken);
        SubItems = await _databaseService.GetSubItemsAsync(cancellationToken);
        Currencies = await _databaseService.GetCurrenciesAsync(cancellationToken);
        Employees = await _databaseService.GetEmployeesAsync(cancellationToken);
    }
}
