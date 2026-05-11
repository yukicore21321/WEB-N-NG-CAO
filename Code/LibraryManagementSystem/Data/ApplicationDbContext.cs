using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<BorrowTicket> BorrowTickets { get; set; }
        public DbSet<BorrowTicketDetail> BorrowTicketDetails { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships for BorrowTicket
            builder.Entity<BorrowTicket>()
                .HasOne(t => t.CreatedByEmployee)
                .WithMany(e => e.CreatedTickets)
                .HasForeignKey(t => t.CreatedByEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BorrowTicket>()
                .HasOne(t => t.ReceivedByEmployee)
                .WithMany(e => e.ReceivedTickets)
                .HasForeignKey(t => t.ReceivedByEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure BorrowTicketDetail relationships
            builder.Entity<BorrowTicketDetail>()
                .HasOne(d => d.BorrowTicket)
                .WithMany(t => t.TicketDetails)
                .HasForeignKey(d => d.BorrowTicketId);

            builder.Entity<BorrowTicketDetail>()
                .HasOne(d => d.Book)
                .WithMany(b => b.BorrowDetails)
                .HasForeignKey(d => d.BookId);

            // Fix Identity Key Length for SQL Server (Comprehensive Fix)
            builder.Entity<ApplicationUser>(entity => {
                entity.Property(m => m.Id).HasMaxLength(128);
            });
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>(entity => {
                entity.Property(m => m.Id).HasMaxLength(128);
            });
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>(entity => {
                entity.Property(m => m.LoginProvider).HasMaxLength(128);
                entity.Property(m => m.ProviderKey).HasMaxLength(128);
                entity.Property(m => m.UserId).HasMaxLength(128);
            });
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>(entity => {
                entity.Property(m => m.UserId).HasMaxLength(128);
                entity.Property(m => m.RoleId).HasMaxLength(128);
            });
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>(entity => {
                entity.Property(m => m.UserId).HasMaxLength(128);
                entity.Property(m => m.LoginProvider).HasMaxLength(128);
                entity.Property(m => m.Name).HasMaxLength(128);
            });
        }
    }
}
