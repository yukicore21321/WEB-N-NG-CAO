using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Apply any pending migrations
            context.Database.Migrate();

            // Seed Roles
            if (roleManager != null)
            {
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }
            }

            // Seed Admin User
            var adminEmail = "admin@library.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Hệ thống Admin",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed Test User
            var userEmail = "user@library.com";
            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    FullName = "Nguyễn Văn Người Dùng",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "User@123");
                await userManager.AddToRoleAsync(user, "User");
            }

            // Seed Categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Công nghệ thông tin", Description = "Sách về lập trình và phần cứng", CreatedAt = DateTime.Now },
                    new Category { Name = "Kinh tế", Description = "Sách về tài chính và quản lý", CreatedAt = DateTime.Now },
                    new Category { Name = "Ngoại ngữ", Description = "Sách học ngôn ngữ", CreatedAt = DateTime.Now },
                    new Category { Name = "Kỹ năng sống", Description = "Sách phát triển bản thân", CreatedAt = DateTime.Now }
                };
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            // Seed Books
            if (!context.Books.Any())
            {
                var itCategory = context.Categories.First(c => c.Name == "Công nghệ thông tin");
                var skillCategory = context.Categories.First(c => c.Name == "Kỹ năng sống");

                var books = new List<Book>
                {
                    new Book { 
                        Title = "Lập trình C# nâng cao", 
                        Author = "Nguyễn Văn A", 
                        Publisher = "NXB Giáo Dục",
                        PublishYear = 2023,
                        Quantity = 10, 
                        AvailableQuantity = 8, 
                        CategoryId = itCategory.Id,
                        Description = "Cuốn sách hướng dẫn lập trình C# từ cơ bản đến nâng cao.",
                        ImageUrl = "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80",
                        CreatedAt = DateTime.Now
                    },
                    new Book { 
                        Title = "ASP.NET Core 9.0 Masterclass", 
                        Author = "Trần Thị B", 
                        Publisher = "NXB Trẻ",
                        PublishYear = 2024,
                        Quantity = 5, 
                        AvailableQuantity = 5, 
                        CategoryId = itCategory.Id,
                        Description = "Xây dựng ứng dụng Web hiện đại với .NET 9.",
                        ImageUrl = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80",
                        CreatedAt = DateTime.Now
                    },
                    new Book { 
                        Title = "Đắc Nhân Tâm", 
                        Author = "Dale Carnegie", 
                        Publisher = "NXB Tổng Hợp",
                        PublishYear = 2022,
                        Quantity = 20, 
                        AvailableQuantity = 15, 
                        CategoryId = skillCategory.Id,
                        Description = "Cuốn sách bán chạy nhất mọi thời đại.",
                        ImageUrl = "https://images.unsplash.com/photo-1589998059171-988d887df646?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80",
                        CreatedAt = DateTime.Now
                    }
                };
                context.Books.AddRange(books);
                context.SaveChanges();
            }

            // Ensure every user has at least one BorrowRecord for testing
            var users = await userManager.Users.ToListAsync();
            var book = context.Books.First();
            foreach (var user in users)
            {
                if (!context.BorrowRecords.Any(r => r.UserId == user.Id))
                {
                    context.BorrowRecords.Add(new BorrowRecord
                    {
                        UserId = user.Id,
                        BookId = book.Id,
                        BorrowDate = DateTime.Now.AddDays(-7),
                        DueDate = DateTime.Now.AddDays(7),
                        Status = BorrowStatus.Borrowed
                    });
                }
            }
            context.SaveChanges();
        }
    }
}
