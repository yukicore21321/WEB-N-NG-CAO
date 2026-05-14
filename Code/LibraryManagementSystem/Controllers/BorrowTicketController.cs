using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    [Route("Admin/BorrowTicket")]
    public class BorrowTicketController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BorrowTicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // LIST
        // =========================

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var tickets = await _context.BorrowTickets

                .Include(x => x.Customer)

                .Include(x => x.TicketDetails)
                    .ThenInclude(td => td.Book)

                .OrderByDescending(x => x.Id)

                .ToListAsync();

            ViewBag.Customers =
                await _context.Customers.ToListAsync();

            ViewBag.Books =
                await _context.Books.ToListAsync();

            return View(
                "~/Views/BorrowTicket/index.cshtml",
                tickets
            );
        }

        // =========================
        // CREATE
        // =========================

        [HttpPost("Create")]
        public async Task<IActionResult> Create(

            BorrowTicket borrowTicket,

            List<int> BookIds

        )
        {
            if (BookIds == null || !BookIds.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            borrowTicket.CreatedAt =
                DateTime.Now;
            borrowTicket.CreatedByEmployeeId = 1;
            borrowTicket.ReceivedByEmployeeId = null;
            borrowTicket.Status =
                "Borrowed";

            _context.BorrowTickets.Add(borrowTicket);

            await _context.SaveChangesAsync();

            // create details
            foreach (var bookId in BookIds)
            {
                var detail =
                    new BorrowTicketDetail
                    {
                        BorrowTicketId = borrowTicket.Id,

                        BookId = bookId,

                    };

                _context.BorrowTicketDetails.Add(detail);

                // update available quantity
                var book =
                    await _context.Books
                        .FirstOrDefaultAsync(x => x.Id == bookId);

                if (book != null)
                {
                    book.AvailableQuantity -= 1;
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT
        // =========================

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(

            BorrowTicket borrowTicket,

            List<int> BookIds

        )
        {
            var existingTicket =
                await _context.BorrowTickets

                    .Include(x => x.TicketDetails)

                    .FirstOrDefaultAsync(x => x.Id == borrowTicket.Id);

            if (existingTicket == null)
            {
                return NotFound();
            }

            // rollback old quantities
            if (existingTicket.TicketDetails != null)
            {
                foreach (var oldDetail in existingTicket.TicketDetails)
                {
                    var oldBook = await _context.Books.FirstOrDefaultAsync(x => x.Id == oldDetail.BookId);
                    if (oldBook != null)
                    {
                        oldBook.AvailableQuantity += 1;
                    }
                }
            }

            // remove old details
            _context.BorrowTicketDetails.RemoveRange(
                existingTicket.TicketDetails
            );

            // update ticket
            existingTicket.CustomerId =
                borrowTicket.CustomerId;
            existingTicket.CreatedByEmployeeId = 1;
            existingTicket.ReceivedByEmployeeId = null;
            existingTicket.BorrowDate =
                borrowTicket.BorrowDate;

            existingTicket.DueDate =
                borrowTicket.DueDate;

            existingTicket.Note =
                borrowTicket.Note;

            existingTicket.Status =
                borrowTicket.Status;

            existingTicket.ReturnDate =
                borrowTicket.ReturnDate;

            // add new details
            foreach (var bookId in BookIds)
            {
                var detail =
                    new BorrowTicketDetail
                    {
                        BorrowTicketId = existingTicket.Id,

                        BookId = bookId,

                    };

                _context.BorrowTicketDetails.Add(detail);

                var book =
                    await _context.Books
                        .FirstOrDefaultAsync(x => x.Id == bookId);

                if (book != null)
                {
                    book.AvailableQuantity -= 1;
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket =
                await _context.BorrowTickets

                    .Include(x => x.TicketDetails)

                    .FirstOrDefaultAsync(x => x.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            // rollback quantity
            if (ticket.TicketDetails != null)
            {
                foreach (var detail in ticket.TicketDetails)
                {
                    var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == detail.BookId);
                    if (book != null)
                    {
                        book.AvailableQuantity += 1;
                    }
                }
            }

            // remove details
            _context.BorrowTicketDetails.RemoveRange(
                ticket.TicketDetails
            );

            // remove ticket
            _context.BorrowTickets.Remove(ticket);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DETAIL
        // =========================

        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var ticket =
                await _context.BorrowTickets

                    .Include(x => x.Customer)

                    .Include(x => x.TicketDetails)
                        .ThenInclude(td => td.Book)

                    .FirstOrDefaultAsync(x => x.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return Json(ticket);
        }

        [HttpPost("MarkReturned/{id}")]
        public async Task<IActionResult> MarkReturned(int id)
        {
            var ticket = await _context.BorrowTickets
                .Include(x => x.TicketDetails)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.Status == "Returned")
            {
                return RedirectToAction(nameof(Index));
            }

            ticket.Status = "Returned";
            ticket.ReturnDate = DateTime.Now;
            ticket.ReceivedByEmployeeId = 1;

            if (ticket.TicketDetails != null)
            {
                foreach (var detail in ticket.TicketDetails)
                {
                    var book = await _context.Books.FindAsync(detail.BookId);
                    if (book != null)
                    {
                        book.AvailableQuantity += 1;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}