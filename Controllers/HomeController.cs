using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp_Anti.Data;
using WebApp_Anti.Models;

namespace WebApp_Anti.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        int pageSize = 50;
        
        // Get total count for pagination
        var totalItems = await _context.SerpProductInstances.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        // Ensure page is within valid range
        page = Math.Max(1, Math.Min(page, totalPages > 0 ? totalPages : 1));

        // Fetch paginated data
        var data = await _context.SerpProductInstances
            .AsNoTracking()
            .OrderBy(x => x.Product) // Fix: determinisic order for pagination
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Pass pagination info to View
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.HasPrevious = page > 1;
        ViewBag.HasNext = page < totalPages;

        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> Submit(List<SerpProductViewModel> items)
    {
        // Filter out items that have approval data
        var submittedItems = items.Where(x => !string.IsNullOrEmpty(x.Approved)).ToList();

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
                existing.Approved = item.Approved;
                existing.ApprovedDate = string.IsNullOrEmpty(item.ApprovedDate) 
                    ? null 
                    : DateTime.Parse(item.ApprovedDate);
                existing.Notes = item.Notes;
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
                    Approved = item.Approved,
                    ApprovedDate = string.IsNullOrEmpty(item.ApprovedDate) 
                        ? null 
                        : DateTime.Parse(item.ApprovedDate),
                    Notes = item.Notes,
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
