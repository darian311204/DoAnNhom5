using DoAn_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn_Backend.Data
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;

        public DatabaseSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Ensure database is created
                await _context.Database.EnsureCreatedAsync();

                // Seed only if database is empty
                if (!_context.Users.Any())
                {
                    await SeedUsers();
                }

                if (!_context.Categories.Any())
                {
                    await SeedCategories();
                }

                if (!_context.Products.Any())
                {
                    await SeedProducts();
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error seeding database: {ex.Message}", ex);
            }
        }

        private async Task SeedUsers()
        {
            // Check if admin already exists
            if (await _context.Users.FirstOrDefaultAsync(u => u.Email == "admin@example.com") != null)
                return;

            var users = new List<User>
            {
                new User
                {
                    FullName = "Admin User",
                    Email = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Phone = "0123456789",
                    Role = "Admin",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new User
                {
                    FullName = "Customer One",
                    Email = "customer1@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("customer123"),
                    Phone = "0987654321",
                    Role = "Customer",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new User
                {
                    FullName = "Customer Two",
                    Email = "customer2@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("customer123"),
                    Phone = "0912345678",
                    Role = "Customer",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                }
            };

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();
        }

        private async Task SeedCategories()
        {
            if (await _context.Categories.AnyAsync())
                return;

            var categories = new List<Category>
            {
                new Category
                {
                    CategoryName = "Áo Nam",
                    Description = "Áo nam thời trang cao cấp",
                    IsActive = true
                },
                new Category
                {
                    CategoryName = "Áo Nữ",
                    Description = "Áo nữ thời trang đa dạng",
                    IsActive = true
                },
                new Category
                {
                    CategoryName = "Quần Nam",
                    Description = "Quần nam phong cách",
                    IsActive = true
                },
                new Category
                {
                    CategoryName = "Quần Nữ",
                    Description = "Quần nữ thời trang",
                    IsActive = true
                },
                new Category
                {
                    CategoryName = "Phụ Kiện",
                    Description = "Phụ kiện thời trang",
                    IsActive = true
                }
            };

            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();
        }

        private async Task SeedProducts()
        {
            if (await _context.Products.AnyAsync())
                return;

            // Get categories to link products
            var aoNam = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == "Áo Nam");
            var aoNu = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == "Áo Nữ");
            var quanNam = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == "Quần Nam");
            var quanNu = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == "Quần Nữ");
            var phuKien = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == "Phụ Kiện");

            var products = new List<Product>
            {
                // Áo Nam
                new Product
                {
                    ProductName = "Áo Sơ Mi Nam Trắng",
                    Price = 350000,
                    Description = "Áo sơ mi nam trắng chất liệu cotton cao cấp, form slim fit",
                    ImageURL = "/images/products/ao-so-mi-nam-trang.jpg",
                    CategoryID = aoNam?.CategoryID ?? 1,
                    Stock = 50,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    ProductName = "Áo Thun Nam Basic",
                    Price = 150000,
                    Description = "Áo thun nam basic màu đen, chất liệu cotton 100%",
                    ImageURL = "/images/products/ao-thun-nam-basic.jpg",
                    CategoryID = aoNam?.CategoryID ?? 1,
                    Stock = 100,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    ProductName = "Áo Polo Nam",
                    Price = 250000,
                    Description = "Áo polo nam chất liệu cotton pique, thoáng mát",
                    ImageURL = "/images/products/ao-polo-nam.jpg",
                    CategoryID = aoNam?.CategoryID ?? 1,
                    Stock = 75,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },

                // Áo Nữ
                new Product
                {
                    ProductName = "Áo Phông Nữ Dáng Rộng",
                    Price = 180000,
                    Description = "Áo phông nữ dáng rộng màu hồng pastel",
                    ImageURL = "/images/products/ao-phong-nu.jpg",
                    CategoryID = aoNu?.CategoryID ?? 2,
                    Stock = 60,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    ProductName = "Áo Sơ Mi Nữ Trắng",
                    Price = 320000,
                    Description = "Áo sơ mi nữ trắng cổ điển, chất liệu chiffon",
                    ImageURL = "/images/products/ao-so-mi-nu.jpg",
                    CategoryID = aoNu?.CategoryID ?? 2,
                    Stock = 45,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },

                // Quần Nam
                new Product
                {
                    ProductName = "Quần Jean Nam Slim",
                    Price = 450000,
                    Description = "Quần jean nam slim fit, màu xanh đậm",
                    ImageURL = "/images/products/quan-jean-nam.jpg",
                    CategoryID = quanNam?.CategoryID ?? 3,
                    Stock = 80,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    ProductName = "Quần Kaki Nam",
                    Price = 380000,
                    Description = "Quần kaki nam màu be, form regular",
                    ImageURL = "/images/products/quan-kaki-nam.jpg",
                    CategoryID = quanNam?.CategoryID ?? 3,
                    Stock = 65,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },

                // Quần Nữ
                new Product
                {
                    ProductName = "Quần Jean Nữ Ống Rộng",
                    Price = 420000,
                    Description = "Quần jean nữ ống rộng màu xanh nhạt",
                    ImageURL = "/images/products/quan-jean-nu.jpg",
                    CategoryID = quanNu?.CategoryID ?? 4,
                    Stock = 70,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    ProductName = "Quần Short Nữ",
                    Price = 220000,
                    Description = "Quần short nữ màu đen, chất liệu cotton",
                    ImageURL = "/images/products/quan-short-nu.jpg",
                    CategoryID = quanNu?.CategoryID ?? 4,
                    Stock = 55,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },

                // Phụ Kiện
                new Product
                {
                    ProductName = "Túi Xách Da",
                    Price = 550000,
                    Description = "Túi xách da thật màu nâu",
                    ImageURL = "/images/products/tui-xach-da.jpg",
                    CategoryID = phuKien?.CategoryID ?? 5,
                    Stock = 30,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    ProductName = "Thắt Lưng Da",
                    Price = 280000,
                    Description = "Thắt lưng da nam màu đen",
                    ImageURL = "/images/products/that-lung-da.jpg",
                    CategoryID = phuKien?.CategoryID ?? 5,
                    Stock = 40,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    ProductName = "Mũ Lưỡi Trai",
                    Price = 180000,
                    Description = "Mũ lưỡi trai màu đen",
                    ImageURL = "/images/products/mu-luoi-trai.jpg",
                    CategoryID = phuKien?.CategoryID ?? 5,
                    Stock = 50,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                }
            };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();
        }
    }
}

