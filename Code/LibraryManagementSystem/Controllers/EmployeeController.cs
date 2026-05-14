using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using LibraryManagementSystem.Services;
using Microsoft.Extensions.Caching.Memory;

namespace LibraryManagementSystem.Controllers
{
    [Route("Admin/Employee")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;

        public EmployeeController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService,
            IMemoryCache cache
        )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _cache = cache;
        }

        // =========================
        // LIST
        // GET: /Admin/Employee
        // =========================

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees

                .OrderByDescending(x => x.Id)

                .ToListAsync();

            return View(
                "~/Views/Employee/index.cshtml",
                employees
            );
        }

        // =========================
        // CREATE
        // POST: /Admin/Employee/Create
        // =========================

        [HttpPost("Create")]
        public async Task<IActionResult> Create(Employee employee)
        {

            // default value
            employee.CreatedAt = DateTime.Now;

            if (string.IsNullOrEmpty(employee.Role))
            {
                employee.Role = "Staff";
            }
            employee.IsActive = employee.IsActive;


            // username default = phone
            if (string.IsNullOrEmpty(employee.Username))
            {
                employee.Username =
                    employee.Phone ?? "";
            }

            // password default
            if (string.IsNullOrEmpty(employee.Password))
            {
                employee.Password = "123456";
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // ĐỒNG BỘ: Tạo tài khoản đăng nhập Identity
            var user = new ApplicationUser
            {
                UserName = employee.Username,
                Email = employee.Email,
                FullName = employee.FullName,
                PhoneNumber = employee.Phone,
                EmailConfirmed = true,
                MustChangePassword = true
            };

            var result = await _userManager.CreateAsync(user, employee.Password ?? "123456");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, employee.Role ?? "Staff");

                // Gửi OTP/Thông báo cho nhân viên mới
                var otp = new Random().Next(100000, 999999).ToString();
                _cache.Set($"OTP_{employee.Email}", otp, TimeSpan.FromMinutes(10));
                await _emailService.SendEmailAsync(employee.Email, "Tài khoản nhân viên mới", otp, employee.FullName);
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT
        // POST: /Admin/Employee/Edit
        // =========================

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(Employee employee)
        {
            var existingEmployee =
                await _context.Employees
                    .FirstOrDefaultAsync(x => x.Id == employee.Id);

            if (existingEmployee == null)
            {
                return NotFound();
            }

            existingEmployee.FullName =
                employee.FullName;

            existingEmployee.Phone =
                employee.Phone;

            existingEmployee.Email =
                employee.Email;
            existingEmployee.IsActive = employee.IsActive;
            // giữ nguyên username/password nếu không truyền
            if (!string.IsNullOrEmpty(employee.Username))
            {
                existingEmployee.Username =
                    employee.Username;
            }

            if (!string.IsNullOrEmpty(employee.Password))
            {
                existingEmployee.Password =
                    employee.Password;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // POST: /Admin/Employee/Delete/1
        // =========================

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee =
                await _context.Employees
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }

}