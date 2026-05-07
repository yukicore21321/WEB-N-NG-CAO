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
        public DbSet<BorrowRecord> BorrowRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships if needed
            builder.Entity<BorrowRecord>()
                .HasOne(b => b.User)
                .WithMany(u => u.BorrowRecords)
                .HasForeignKey(b => b.UserId);

            builder.Entity<BorrowRecord>()
                .HasOne(b => b.Book)
                .WithMany(bk => bk.BorrowRecords)
                .HasForeignKey(b => b.BookId);
        }
    }
}
