using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Controllers
{
    [Route("Admin/Customer")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(
            ApplicationDbContext context,
            IEmailService emailService,
            IMemoryCache cache,
            UserManager<ApplicationUser> userManager
        )
        {
            _context = context;
            _emailService = emailService;
            _cache = cache;
            _userManager = userManager;
        }

        // =========================
        // INDEX
        // =========================

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return View(
                "Views/Customer/Index.cshtml",
                customers
            );
        }

        // =========================
        // CREATE
        // =========================

        [HttpPost("Create")]
        public async Task<IActionResult> Create(
            Customer customer, string OtpCode
        )
        {
            // Kiểm tra mã OTP
            if (_cache.TryGetValue($"OTP_{customer.Email}", out string? cachedOtp))
            {
                if (cachedOtp != OtpCode)
                {
                    ModelState.AddModelError("", "Mã OTP không chính xác.");
                    return View("Views/Customer/Index.cshtml", await _context.Customers.ToListAsync());
                }
            }
            else
            {
                ModelState.AddModelError("", "Mã OTP đã hết hạn hoặc không tồn tại.");
                return View("Views/Customer/Index.cshtml", await _context.Customers.ToListAsync());
            }

            customer.CreatedAt = DateTime.Now;
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Xóa OTP sau khi dùng xong
            _cache.Remove($"OTP_{customer.Email}");

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT
        // =========================

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(
            Customer customer
        )
        {
            var oldCustomer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == customer.Id);

            if (oldCustomer != null)
            {
                oldCustomer.FullName =
                    customer.FullName;

                oldCustomer.Phone =
                    customer.Phone;

                oldCustomer.Email =
                    customer.Email;

                oldCustomer.Address =
                    customer.Address;

                oldCustomer.DateOfBirth =
                    customer.DateOfBirth;

                oldCustomer.Gender =
                    customer.Gender;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (customer != null)
            {
                _context.Customers.Remove(customer);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // AJAX: SEND OTP
        // =========================

        [HttpPost("SendOTP")]
        public async Task<IActionResult> SendOTP([FromBody] OtpRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { success = false, message = "Email không được để trống." });
            }

            try
            {
                var otp = new Random().Next(100000, 999999).ToString();
                _cache.Set($"OTP_{request.Email}", otp, TimeSpan.FromMinutes(10));

                await _emailService.SendEmailAsync(request.Email, "Mã xác thực đăng ký khách hàng", otp);

                return Ok(new { success = true, message = "Mã OTP đã được gửi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi gửi mail: " + ex.Message });
            }
        }
    }

    public class OtpRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}