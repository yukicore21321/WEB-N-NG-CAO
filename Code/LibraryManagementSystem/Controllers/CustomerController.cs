using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Route("Admin/Customer")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(
            ApplicationDbContext context
        )
        {
            _context = context;
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