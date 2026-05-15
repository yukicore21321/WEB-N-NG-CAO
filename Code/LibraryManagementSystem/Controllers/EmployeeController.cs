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
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
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
            
            // username default = phone
            if (string.IsNullOrEmpty(employee.Username))
            {
                employee.Username = employee.Phone ?? "";
            }

            // password default
            if (string.IsNullOrEmpty(employee.Password))
            {
                employee.Password = "123456";
            }

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Keys.SelectMany(key => ModelState[key].Errors.Select(e => $"{key}: {e.ErrorMessage}")));
                ModelState.AddModelError("", "Dữ liệu không hợp lệ: " + errors);
                return View("~/Views/Employee/index.cshtml", await _context.Employees.OrderByDescending(x => x.Id).ToListAsync());
            }

            // check if username or email already exists in Identity or Employees table
            var existingIdentityUser = await _userManager.FindByNameAsync(employee.Username) ?? 
                                        (!string.IsNullOrEmpty(employee.Email) ? await _userManager.FindByEmailAsync(employee.Email) : null);
            
            var existingEmployee = await _context.Employees.AnyAsync(e => e.Username == employee.Username || (!string.IsNullOrEmpty(employee.Email) && e.Email == employee.Email));

            if (existingIdentityUser != null || existingEmployee)
            {
                ModelState.AddModelError("", "Tài khoản hoặc Email đã tồn tại trong hệ thống.");
                return View("~/Views/Employee/index.cshtml", await _context.Employees.OrderByDescending(x => x.Id).ToListAsync());
            }

            // ĐỒNG BỘ: Tạo tài khoản đăng nhập Identity TRƯỚC
            var user = new ApplicationUser
            {
                UserName = employee.Username,
                Email = employee.Email,
                FullName = employee.FullName,
                PhoneNumber = employee.Phone,
                EmailConfirmed = true,
                MustChangePassword = true // Khôi phục yêu cầu đổi mật khẩu
            };

            var result = await _userManager.CreateAsync(user, employee.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, employee.Role);

                // Sau khi Identity thành công mới tạo bản ghi Employee
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // Gửi OTP/Thông báo cho nhân viên mới
                try {
                    var otp = new Random().Next(100000, 999999).ToString();
                    _cache.Set($"OTP_ChangePwd_{user.Id}", otp, TimeSpan.FromMinutes(10)); // Set OTP cho lần đăng nhập đầu tiên
                    await _emailService.SendEmailAsync(employee.Email, "Tài khoản nhân viên mới", $"Mã xác thực đăng nhập của bạn là: {otp}", employee.FullName);
                } catch {
                    // Ignore email errors in dev
                }

                return RedirectToAction(nameof(Index));
            }

            // Nếu thất bại
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            
            return View("~/Views/Employee/index.cshtml", await _context.Employees.OrderByDescending(x => x.Id).ToListAsync());
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
                existingEmployee.Password = employee.Password;
                
                // Cập nhật mật khẩu trong Identity
                var user = await _userManager.FindByNameAsync(existingEmployee.Username);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, employee.Password);
                }
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