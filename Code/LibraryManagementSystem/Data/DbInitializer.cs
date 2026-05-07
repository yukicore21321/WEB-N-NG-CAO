using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create database if it doesn't exist
            context.Database.EnsureCreated();

            // Seed Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
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
                    new Category { Name = "Công nghệ thông tin" },
                    new Category { Name = "Kinh tế" },
                    new Category { Name = "Ngoại ngữ" },
                    new Category { Name = "Kỹ năng sống" }
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
                        ISBN = "123456789", 
                        TotalCopies = 10, 
                        AvailableCopies = 8, 
                        CategoryId = itCategory.Id,
                        Description = "Cuốn sách hướng dẫn lập trình C# từ cơ bản đến nâng cao.",
                        ImagePath = "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80"
                    },
                    new Book { 
                        Title = "ASP.NET Core 9.0 Masterclass", 
                        Author = "Trần Thị B", 
                        ISBN = "987654321", 
                        TotalCopies = 5, 
                        AvailableCopies = 5, 
                        CategoryId = itCategory.Id,
                        Description = "Xây dựng ứng dụng Web hiện đại với .NET 9.",
                        ImagePath = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80"
                    },
                    new Book { 
                        Title = "Đắc Nhân Tâm", 
                        Author = "Dale Carnegie", 
                        ISBN = "111222333", 
                        TotalCopies = 20, 
                        AvailableCopies = 15, 
                        CategoryId = skillCategory.Id,
                        Description = "Cuốn sách bán chạy nhất mọi thời đại.",
                        ImagePath = "https://images.unsplash.com/photo-1589998059171-988d887df646?ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=80"
                    }
                };
                context.Books.AddRange(books);
                context.SaveChanges();
            }
        }
    }
}
