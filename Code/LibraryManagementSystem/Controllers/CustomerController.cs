using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Services;
using Microsoft.Extensions.Caching.Memory;

namespace LibraryManagementSystem.Controllers
{
    [Route("Admin/Customer")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;

        public CustomerController(
            ApplicationDbContext context,
            IEmailService emailService,
            IMemoryCache cache
        )
        {
            _context = context;
            _emailService = emailService;
            _cache = cache;
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
            Customer customer
        )
        {
            customer.CreatedAt = DateTime.Now;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Gửi mail thông báo cho khách hàng mới (nếu có email)
            if (!string.IsNullOrEmpty(customer.Email))
            {
                var otp = new Random().Next(100000, 999999).ToString();
                // Lưu vào cache nếu cần xác thực sau này, ở đây ta gửi mail chào mừng
                _cache.Set($"OTP_{customer.Email}", otp, TimeSpan.FromMinutes(10));
                await _emailService.SendEmailAsync(customer.Email, "Chào mừng bạn đến với BookWorm", otp, customer.FullName);
            }

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
    }
}