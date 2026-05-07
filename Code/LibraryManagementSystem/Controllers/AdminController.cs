using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    // [Authorize(Roles = "Admin")] // Tạm thời comment để test UI dễ dàng
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Tổng quan hệ thống";
            return View();
        }

        public IActionResult Books()
        {
            ViewData["Title"] = "Quản lý Sách";
            return View();
        }

        public IActionResult BorrowRecords()
        {
            ViewData["Title"] = "Quản lý Mượn / Trả";
            return View();
        }

        public IActionResult Users()
        {
            ViewData["Title"] = "Quản lý Người dùng";
            return View();
        }

        public IActionResult Categories()
        {
            ViewData["Title"] = "Quản lý Thể loại";
            return View();
        }

        public IActionResult CreateBook()
        {
            ViewData["Title"] = "Thêm sách mới";
            return View();
        }
    }
}
