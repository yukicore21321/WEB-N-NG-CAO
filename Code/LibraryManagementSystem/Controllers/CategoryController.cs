using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

// using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers
{
    // [Authorize]v

    [Route("Admin/Categories")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Staff")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // CATEGORY LIST
        // GET: /Admin/Categories
        // =========================

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            Console.WriteLine(
                $"Fetched {categories.Count} categories from database."
            );

            return View(
                "~/Views/Category/index.cshtml",
                categories
            );
        }

        // =========================
        // CREATE CATEGORY
        // POST: /Admin/Categories/Create
        // =========================

        [HttpPost("Create")]
        public async Task<IActionResult> Create(Category category)
        {
            if (category.Name != null)
            {
                category.CreatedAt = DateTime.Now;

                _context.Categories.Add(category);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            var categories = await _context.Categories
                .ToListAsync();
            return View(
                "~/Views/Category/index.cshtml",
                categories
            );
        }

        // =========================
        // EDIT CATEGORY
        // POST: /Admin/Categories/Edit
        // =========================

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                var existingCategory =
                    await _context.Categories.FindAsync(category.Id);

                if (existingCategory == null)
                {
                    return NotFound();
                }

                existingCategory.Name = category.Name;

                existingCategory.Description =
                    category.Description;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE CATEGORY
        // POST: /Admin/Categories/Delete/1
        // =========================

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories
                .FindAsync(id);

            if (category != null)
            {
                _context.Categories.Remove(category);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // CATEGORY DETAIL
        // GET: /Admin/Categories/Detail/1
        // =========================

        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return Json(category);
        }
    }
}