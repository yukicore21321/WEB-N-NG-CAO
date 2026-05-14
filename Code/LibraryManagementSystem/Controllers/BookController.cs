using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    [Route("Admin/Book")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public BookController(
            ApplicationDbContext context,
            IWebHostEnvironment environment
        )
        {
            _context = context;
            _environment = environment;
        }

        // INDEX
        [HttpGet("")]
        public async Task<IActionResult> Index(string? keyword)
        {
            ViewBag.Categories =
                await _context.Categories.ToListAsync();

            var query = _context.Books
    .Include(x => x.Category)
    .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x =>

                    x.Title.Contains(keyword)
                );
            }

            var books = await query

                .OrderByDescending(x => x.Id)

                .ToListAsync();
            ViewBag.Keyword = keyword;
            return View(
                "Views/Book/index.cshtml",
                books
            );
        }

        // CREATE
        [HttpPost("Create")]
        public async Task<IActionResult> Create(
            Book book,
            IFormFile? logoFile
        )
        {
            if (logoFile != null)
            {
                string fileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(logoFile.FileName);

                string uploadFolder =
                    Path.Combine(
                        _environment.WebRootPath,
                        "images",
                        "books"
                    );

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string filePath =
                    Path.Combine(uploadFolder, fileName);

                using (var stream =
                       new FileStream(filePath, FileMode.Create))
                {
                    await logoFile.CopyToAsync(stream);
                }

                book.ImageUrl =
                    "/images/books/" + fileName;
            }

            book.CreatedAt = DateTime.Now;

            book.AvailableQuantity =
                book.Quantity;

            _context.Books.Add(book);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // EDIT
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(
            Book book,
            IFormFile? logoFile
        )
        {
            var oldBook = await _context.Books
                .FirstOrDefaultAsync(x => x.Id == book.Id);

            if (oldBook != null)
            {
                oldBook.Title = book.Title;
                oldBook.Description = book.Description;
                oldBook.Author = book.Author;
                oldBook.Publisher = book.Publisher;
                oldBook.PublishYear = book.PublishYear;
                oldBook.Quantity = book.Quantity;
                oldBook.AvailableQuantity =
                    book.AvailableQuantity;
                oldBook.CategoryId = book.CategoryId;

                // upload image
                if (logoFile != null)
                {
                    // XOÁ ẢNH CŨ
                    if (!string.IsNullOrEmpty(oldBook.ImageUrl))
                    {
                        string oldImagePath =
                            Path.Combine(
                                _environment.WebRootPath,
                                oldBook.ImageUrl.TrimStart('/')
                                    .Replace("/", Path.DirectorySeparatorChar.ToString())
                            );

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // LƯU ẢNH MỚI
                    string fileName =
                        Guid.NewGuid().ToString()
                        + Path.GetExtension(logoFile.FileName);

                    string uploadFolder =
                        Path.Combine(
                            _environment.WebRootPath,
                            "images",
                            "books"
                        );

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    string filePath =
                        Path.Combine(uploadFolder, fileName);

                    using (var stream =
                           new FileStream(filePath, FileMode.Create))
                    {
                        await logoFile.CopyToAsync(stream);
                    }

                    oldBook.ImageUrl =
                        "/images/books/" + fileName;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(x => x.Id == id);

            if (book != null)
            {
                // XOÁ ẢNH
                if (!string.IsNullOrEmpty(book.ImageUrl))
                {
                    string imagePath =
                        Path.Combine(
                            _environment.WebRootPath,
                            book.ImageUrl.TrimStart('/')
                                .Replace("/", Path.DirectorySeparatorChar.ToString())
                        );

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // XOÁ DB
                _context.Books.Remove(book);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}