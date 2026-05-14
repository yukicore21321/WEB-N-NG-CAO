using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    [Route("Admin/Employee")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
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