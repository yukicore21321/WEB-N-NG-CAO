using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using LibraryManagementSystem.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService,
            IMemoryCache cache)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _cache = cache;
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

        public IActionResult Branches()
        {
            ViewData["Title"] = "Quản lý Chi nhánh";
            return View();
        }

        public IActionResult CreateBook()
        {
            ViewData["Title"] = "Thêm sách mới";
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult OTP()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string firstName, string lastName, string email, string phone, string username, string password, string role)
        {
            if (string.IsNullOrEmpty(role)) role = "User";

            var user = new ApplicationUser
            {
                UserName = username,
                Email = email,
                FullName = $"{firstName} {lastName}",
                MustChangePassword = (role != "User"),
                EmailConfirmed = true // Admin cấp nên có thể confirm luôn hoặc bắt confirm qua mail
            };

            var result = await _userManager.CreateAsync(user, password ?? "Temp@123");
            if (result.Succeeded)
            {
                // Gán quyền
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                await _userManager.AddToRoleAsync(user, role);

                // Nếu là Staff hoặc Admin, tạo bản ghi bên bảng Employee để làm việc
                if (role == "Staff" || role == "Admin")
                {
                    var employee = new Employee
                    {
                        Username = username,
                        Password = password ?? "Temp@123", // Lưu hash hoặc pass tạm
                        FullName = user.FullName,
                        Email = email,
                        Phone = phone,
                        Role = role,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };
                    _context.Employees.Add(employee);
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = $"Đã cấp tài khoản {role} thành công cho {user.FullName}";
                return RedirectToAction("Users");
            }

            ViewBag.Error = string.Join(", ", result.Errors.Select(e => e.Description));
            return View();
        }
    }
}
