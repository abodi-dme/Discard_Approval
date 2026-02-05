using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp_Anti.Data;
using WebApp_Anti.Models;

namespace WebApp_Anti.Controllers;

public class HomeController : Controller
{
    private const int PageSize = 50;
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index(
        int managerPage = 1,
        int finalPage = 1,
        int approvedPage = 1,
        string? filterProduct = null,
        string? filterAssetTag = null,
        string? filterStatus = null,
        string? filterManufacturer = null,
        string? filterModel = null,
        string? filterSerialNumber = null,
        string? filterWarehouse = null,
        string? filterDiscardReason = null,
        string? filterManagerApproved = null,
        string? filterFinalApproved = null)
    {
        try
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            ViewBag.UserRole = GetUserRole();
            ViewBag.CanSubmitManager = CanSubmitManager();
            ViewBag.CanSubmitFinal = CanSubmitFinal();

            // Base query - Filter by Status "Needs Repairing"
            var query = _context.DiscardApprovalInputs
                .Where(x => x.Status == "Needs Repairing");

            // Get distinct values for all filter dropdowns (before applying filters)
            var allData = await query.AsNoTracking().ToListAsync();
            
            ViewBag.ProductOptions = allData
                .Select(x => x.Product)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            
            ViewBag.AssetTagOptions = allData
                .Select(x => x.AssetTag)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            
            ViewBag.StatusOptions = allData
                .Select(x => x.Status)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            
            ViewBag.ManufacturerOptions = allData
                .Select(x => x.Manufacturer)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            
            ViewBag.ModelOptions = allData
                .Select(x => x.Model)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            
            ViewBag.SerialNumberOptions = allData
                .Select(x => x.ManufacturerSerialNumber)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            
            ViewBag.WarehouseOptions = allData
                .Select(x => x.WarehouseName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            
            ViewBag.DiscardReasonOptions = allData
                .Select(x => x.DiscardReason)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filterProduct))
                query = query.Where(x => x.Product == filterProduct);
            
            if (!string.IsNullOrWhiteSpace(filterAssetTag))
                query = query.Where(x => x.AssetTag == filterAssetTag);
            
            if (!string.IsNullOrWhiteSpace(filterStatus))
                query = query.Where(x => x.Status == filterStatus);
            
            if (!string.IsNullOrWhiteSpace(filterManufacturer))
                query = query.Where(x => x.Manufacturer == filterManufacturer);
            
            if (!string.IsNullOrWhiteSpace(filterModel))
                query = query.Where(x => x.Model == filterModel);
            
            if (!string.IsNullOrWhiteSpace(filterSerialNumber))
                query = query.Where(x => x.ManufacturerSerialNumber == filterSerialNumber);
            
            if (!string.IsNullOrWhiteSpace(filterWarehouse))
                query = query.Where(x => x.WarehouseName == filterWarehouse);
            
            if (!string.IsNullOrWhiteSpace(filterDiscardReason))
                query = query.Where(x => x.DiscardNotes == filterDiscardReason);

            var managerTable = await BuildTableAsync(
                "manager",
                managerPage,
                filterProduct,
                filterAssetTag,
                filterStatus,
                filterManufacturer,
                filterModel,
                filterSerialNumber,
                filterWarehouse,
                filterDiscardReason,
                filterManagerApproved,
                filterFinalApproved);

            var finalTable = await BuildTableAsync(
                "final",
                finalPage,
                filterProduct,
                filterAssetTag,
                filterStatus,
                filterManufacturer,
                filterModel,
                filterSerialNumber,
                filterWarehouse,
                filterDiscardReason,
                filterManagerApproved,
                filterFinalApproved);

            var approvedTable = await BuildTableAsync(
                "approved",
                approvedPage,
                filterProduct,
                filterAssetTag,
                filterStatus,
                filterManufacturer,
                filterModel,
                filterSerialNumber,
                filterWarehouse,
                filterDiscardReason,
                filterManagerApproved,
                filterFinalApproved);

            var totalCostValues = await query.AsNoTracking()
                .Select(x => x.UnitCostLastPrice)
                .ToListAsync();

            var totalDiscardCost = totalCostValues
                .Select(x => decimal.TryParse(x, out var v) ? v : 0m)
                .Sum();

            var approvedCostValues = await query.AsNoTracking()
                .Where(x => x.ManagerApproved == "Yes" && x.FinalApproved == "Yes")
                .Select(x => x.UnitCostLastPrice)
                .ToListAsync();

            var approvedDiscardCost = approvedCostValues
                .Select(x => decimal.TryParse(x, out var v) ? v : 0m)
                .Sum();

            var pageModel = new DiscardApprovalPageViewModel
            {
                ManagerTable = managerTable,
                FinalTable = finalTable,
                ApprovedTable = approvedTable,
                TotalDiscardCost = totalDiscardCost,
                ApprovedDiscardCost = approvedDiscardCost
            };

            return View(pageModel);
        }
        catch (Exception ex)
        {
             System.IO.File.WriteAllText("error_log.txt", ex.ToString());
             throw;
        }
    }

[HttpGet]
public async Task<IActionResult> TablePartial(
    int page = 1,
    string stage = "manager",
    string? filterProduct = null,
    string? filterAssetTag = null,
    string? filterStatus = null,
    string? filterManufacturer = null,
    string? filterModel = null,
    string? filterSerialNumber = null,
    string? filterWarehouse = null,
    string? filterDiscardReason = null,
    string? filterManagerApproved = null,
    string? filterFinalApproved = null)
{
    if (!IsLoggedIn())
        return Unauthorized();

    ViewBag.UserRole = GetUserRole();
    ViewBag.CanSubmitManager = CanSubmitManager();
    ViewBag.CanSubmitFinal = CanSubmitFinal();

    var table = await BuildTableAsync(
        stage,
        page,
        filterProduct,
        filterAssetTag,
        filterStatus,
        filterManufacturer,
        filterModel,
        filterSerialNumber,
        filterWarehouse,
        filterDiscardReason,
        filterManagerApproved,
        filterFinalApproved);

    return PartialView("_DiscardApprovalAjax", table);
}

    [HttpPost]
    public async Task<IActionResult> Submit(List<SerpProductViewModel> items, string stage)
    {
        if (!IsLoggedIn())
            return Unauthorized();

        var isFinalStage = string.Equals(stage, "final", StringComparison.OrdinalIgnoreCase);
        if (isFinalStage && !CanSubmitFinal())
            return Forbid();

        if (!isFinalStage && !CanSubmitManager())
            return Forbid();

        // Filter out items that have approval data
        var submittedItems = isFinalStage
            ? items.Where(x => !string.IsNullOrEmpty(x.FinalApproved)).ToList()
            : items.Where(x => !string.IsNullOrEmpty(x.ManagerApproved)).ToList();

        submittedItems = submittedItems
            .Where(x => !string.IsNullOrWhiteSpace(x.DiscardReason))
            .Where(x => !string.Equals(x.DiscardReason, "Other", StringComparison.OrdinalIgnoreCase) ||
                        !string.IsNullOrWhiteSpace(isFinalStage ? x.FinalNotes : x.ManagerNotes))
            .ToList();

        int updatedCount = 0;
        int insertedCount = 0;

        foreach (var item in submittedItems)
        {
            if (string.IsNullOrEmpty(item.AssetTag))
                continue; // Skip if no AssetTag

            // Check if record exists
            var existing = await _context.DiscardApprovals
                .FirstOrDefaultAsync(d => d.AssetTag == item.AssetTag);

            if (existing != null)
            {
                // UPDATE existing record
                existing.Product = item.Product;
                existing.DiscardReason = item.DiscardReason;
                if (isFinalStage)
                {
                    existing.FinalApproved = item.FinalApproved;
                    existing.FinalApprovedDate = string.IsNullOrEmpty(item.FinalApprovedDate)
                        ? null
                        : DateTime.Parse(item.FinalApprovedDate);
                    existing.FinalNotes = item.FinalNotes;
                }
                else
                {
                    existing.ManagerApproved = item.ManagerApproved;
                    existing.ManagerApprovedDate = string.IsNullOrEmpty(item.ManagerApprovedDate)
                        ? null
                        : DateTime.Parse(item.ManagerApprovedDate);
                    existing.ManagerNotes = item.ManagerNotes;
                }
                existing.Type = item.Type;
                existing.MgrWarehouse = item.MgrWarehouse;
                existing.MgrDfo = item.MgrDfo;
                existing.MgrVp = item.MgrVp;
                existing.UpdatedAt = DateTime.Now;
                updatedCount++;
            }
            else
            {
                // INSERT new record
                var newApproval = new DiscardApproval
                {
                    AssetTag = item.AssetTag,
                    Product = item.Product,
                    DiscardReason = item.DiscardReason,
                    ManagerApproved = isFinalStage ? null : item.ManagerApproved,
                    ManagerApprovedDate = isFinalStage || string.IsNullOrEmpty(item.ManagerApprovedDate)
                        ? null
                        : DateTime.Parse(item.ManagerApprovedDate),
                    FinalApproved = isFinalStage ? item.FinalApproved : null,
                    FinalApprovedDate = !isFinalStage || string.IsNullOrEmpty(item.FinalApprovedDate)
                        ? null
                        : DateTime.Parse(item.FinalApprovedDate),
                    ManagerNotes = isFinalStage ? null : item.ManagerNotes,
                    FinalNotes = isFinalStage ? item.FinalNotes : null,
                    Type = item.Type,
                    MgrWarehouse = item.MgrWarehouse,
                    MgrDfo = item.MgrDfo,
                    MgrVp = item.MgrVp,
                    UpdatedAt = DateTime.Now
                };
                _context.DiscardApprovals.Add(newApproval);
                insertedCount++;
            }
        }

        // Save all changes to database
        await _context.SaveChangesAsync();

        TempData["Message"] = $"Successfully processed {submittedItems.Count} items. " +
                             $"({insertedCount} new, {updatedCount} updated)";

        return RedirectToAction("Index");
    }

    private bool IsLoggedIn()
    {
        return !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("UserName"));
    }

    private string GetUserRole()
    {
        return HttpContext.Session.GetString("UserRole") ?? string.Empty;
    }

    private bool CanSubmitManager()
    {
        var role = GetUserRole();
        return role == "Manager" || role == "VP" || role == "Admin";
    }

    private bool CanSubmitFinal()
    {
        var role = GetUserRole();
        return role == "VP" || role == "Admin";
    }

    private IQueryable<DiscardApprovalInput> ApplyFilters(
        IQueryable<DiscardApprovalInput> query,
        string? filterProduct,
        string? filterAssetTag,
        string? filterStatus,
        string? filterManufacturer,
        string? filterModel,
        string? filterSerialNumber,
        string? filterWarehouse,
        string? filterDiscardReason,
        string? filterManagerApproved,
        string? filterFinalApproved)
    {
        if (!string.IsNullOrWhiteSpace(filterProduct))
            query = query.Where(x => x.Product == filterProduct);

        if (!string.IsNullOrWhiteSpace(filterAssetTag))
            query = query.Where(x => x.AssetTag == filterAssetTag);

        if (!string.IsNullOrWhiteSpace(filterStatus))
            query = query.Where(x => x.Status == filterStatus);

        if (!string.IsNullOrWhiteSpace(filterManufacturer))
            query = query.Where(x => x.Manufacturer == filterManufacturer);

        if (!string.IsNullOrWhiteSpace(filterModel))
            query = query.Where(x => x.Model == filterModel);

        if (!string.IsNullOrWhiteSpace(filterSerialNumber))
            query = query.Where(x => x.ManufacturerSerialNumber == filterSerialNumber);

        if (!string.IsNullOrWhiteSpace(filterWarehouse))
            query = query.Where(x => x.WarehouseName == filterWarehouse);

        if (!string.IsNullOrWhiteSpace(filterDiscardReason))
            query = query.Where(x => x.DiscardReason == filterDiscardReason);

        if (!string.IsNullOrWhiteSpace(filterManagerApproved))
            query = query.Where(x => x.ManagerApproved == filterManagerApproved);

        if (!string.IsNullOrWhiteSpace(filterFinalApproved))
            query = query.Where(x => x.FinalApproved == filterFinalApproved);

        return query;
    }

    private IQueryable<DiscardApprovalInput> ApplyStageFilter(IQueryable<DiscardApprovalInput> query, string stage)
    {
        if (string.Equals(stage, "final", StringComparison.OrdinalIgnoreCase))
        {
            // Final tab should show items pending final approval and already final approved
            return query.Where(x => x.ManagerApproved == "Yes");
        }

        if (string.Equals(stage, "approved", StringComparison.OrdinalIgnoreCase))
        {
            return query.Where(x => x.FinalApproved == "Yes");
        }

        // Manager tab should show all items (including approved) for visibility
        return query;
    }

    private async Task<DiscardApprovalTableViewModel> BuildTableAsync(
        string stage,
        int page,
        string? filterProduct,
        string? filterAssetTag,
        string? filterStatus,
        string? filterManufacturer,
        string? filterModel,
        string? filterSerialNumber,
        string? filterWarehouse,
        string? filterDiscardReason,
        string? filterManagerApproved,
        string? filterFinalApproved)
    {
        var baseQuery = _context.DiscardApprovalInputs
            .Where(x => x.Status == "Needs Repairing");

        baseQuery = ApplyStageFilter(baseQuery, stage);

        var optionsData = await baseQuery.AsNoTracking().ToListAsync();

        var query = ApplyFilters(
            baseQuery,
            filterProduct,
            filterAssetTag,
            filterStatus,
            filterManufacturer,
            filterModel,
            filterSerialNumber,
            filterWarehouse,
            filterDiscardReason,
            filterManagerApproved,
            filterFinalApproved);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
        page = Math.Max(1, Math.Min(page, totalPages > 0 ? totalPages : 1));

        var items = await query.AsNoTracking()
            .OrderBy(x => x.Product)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return new DiscardApprovalTableViewModel
        {
            Stage = stage,
            Items = items,
            Pager = new PagerViewModel
            {
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems,
                HasPrevious = page > 1,
                HasNext = page < totalPages
            },
            ProductOptions = optionsData
                .Select(x => x.Product)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!)
                .Distinct()
                .OrderBy(x => x)
                .ToList(),
            AssetTagOptions = optionsData
                .Select(x => x.AssetTag)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!)
                .Distinct()
                .OrderBy(x => x)
                .ToList(),
            StatusOptions = optionsData
                .Select(x => x.Status)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!)
                .Distinct()
                .OrderBy(x => x)
                .ToList(),
            ManufacturerOptions = optionsData
                .Select(x => x.Manufacturer)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!)
                .Distinct()
                .OrderBy(x => x)
                .ToList(),
            ModelOptions = optionsData
                .Select(x => x.Model)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!)
                .Distinct()
                .OrderBy(x => x)
                .ToList(),
            SerialNumberOptions = optionsData
                .Select(x => x.ManufacturerSerialNumber)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!)
                .Distinct()
                .OrderBy(x => x)
                .ToList(),
            WarehouseOptions = optionsData
                .Select(x => x.WarehouseName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!)
                .Distinct()
                .OrderBy(x => x)
                .ToList(),
            DiscardReasonOptions = optionsData
                .Select(x => x.DiscardReason)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!)
                .Distinct()
                .OrderBy(x => x)
                .ToList(),
            FilterProduct = filterProduct,
            FilterAssetTag = filterAssetTag,
            FilterStatus = filterStatus,
            FilterManufacturer = filterManufacturer,
            FilterModel = filterModel,
            FilterSerialNumber = filterSerialNumber,
            FilterWarehouse = filterWarehouse,
            FilterDiscardReason = filterDiscardReason,
            FilterManagerApproved = filterManagerApproved,
            FilterFinalApproved = filterFinalApproved
        };
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
