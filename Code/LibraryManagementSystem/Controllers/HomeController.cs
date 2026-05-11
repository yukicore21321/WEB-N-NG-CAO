using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToAction("Login", "Account");

        ViewBag.TotalBorrowed = await _context.BorrowRecords
            .CountAsync(r => r.UserId == userId && r.Status == BorrowStatus.Borrowed);
        
        ViewBag.TotalReturned = await _context.BorrowRecords
            .CountAsync(r => r.UserId == userId && r.Status == BorrowStatus.Returned);

        return View();
    }

    public async Task<IActionResult> UserDashboard()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return RedirectToAction("Login", "Account");

        ViewBag.TotalBorrowed = await _context.BorrowRecords
            .CountAsync(r => r.UserId == userId && r.Status == BorrowStatus.Borrowed);
        
        ViewBag.TotalReturned = await _context.BorrowRecords
            .CountAsync(r => r.UserId == userId && r.Status == BorrowStatus.Returned);

        return View();
    }

    public async Task<IActionResult> BorrowedBooks()
    {
        var userId = _userManager.GetUserId(User);
        var records = await _context.BorrowRecords
            .Include(r => r.Book)
            .Where(r => r.UserId == userId && r.Status == BorrowStatus.Borrowed)
            .ToListAsync();
        return View(records);
    }

    public async Task<IActionResult> ReturnedBooks()
    {
        var userId = _userManager.GetUserId(User);
        var records = await _context.BorrowRecords
            .Include(r => r.Book)
            .Where(r => r.UserId == userId && r.Status == BorrowStatus.Returned)
            .ToListAsync();
        return View(records);
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
