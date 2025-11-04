using DoAn_Backend.Data;
using DoAn_Backend.DTOs;
using DoAn_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DoAn_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string? _adminToken;

        public SeedDataController(
            ApplicationDbContext context,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;

            // Set base address for API calls
            var baseUrl = _configuration["ApiBaseUrl"] ?? "http://localhost:5090/api";
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [HttpPost]
        public async Task<IActionResult> SeedData()
        {
            try
            {
                // Ensure database is created
                await _context.Database.EnsureCreatedAsync();

                // Check if database needs seeding
                var hasUsers = await _context.Users.AnyAsync();

                if (hasUsers)
                {
                    return Ok(new { message = "Database already contains data, skipping seed", seeded = false });
                }

                // First, login as admin or register admin user
                await AuthenticateAsAdmin();

                // Seed users through API
                await SeedUsers();

                // Seed categories through API
                await SeedCategories();

                // Seed products through API
                await SeedProducts();

                return Ok(new { message = "Database seeded successfully via API", seeded = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error seeding database via API: {ex.Message}", error = ex.ToString() });
            }
        }

        private async Task AuthenticateAsAdmin()
        {
            try
            {
                // Try to login as admin
                var loginDto = new LoginDto
                {
                    Email = "admin@example.com",
                    Password = "admin123"
                };

                var response = await _httpClient.PostAsJsonAsync("Auth/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    _adminToken = result?.Token;

                    if (!string.IsNullOrEmpty(_adminToken))
                    {
                        _httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", _adminToken);
                        return;
                    }
                }

                // If login fails, register admin user first
                var registerDto = new RegisterDto
                {
                    FullName = "Admin User",
                    Email = "admin@example.com",
                    Password = "admin123",
                    Phone = "0123456789"
                };

                response = await _httpClient.PostAsJsonAsync("Auth/register", registerDto);
                if (response.IsSuccessStatusCode)
                {
                    // After registration, login
                    response = await _httpClient.PostAsJsonAsync("Auth/login", loginDto);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                        _adminToken = result?.Token;

                        if (!string.IsNullOrEmpty(_adminToken))
                        {
                            _httpClient.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Bearer", _adminToken);
                        }
                    }

                    // Update user role to Admin directly in DB (since register creates Customer)
                    var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "admin@example.com");
                    if (adminUser != null)
                    {
                        adminUser.Role = "Admin";
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not authenticate as admin: {ex.Message}");
            }
        }

        private async Task SeedUsers()
        {
            // Check if users already exist
            if (await _context.Users.AnyAsync(u => u.Email == "customer1@example.com"))
                return;

            // Register customer users through API
            var customers = new[]
            {
                new RegisterDto
                {
                    FullName = "Customer One",
                    Email = "customer1@example.com",
                    Password = "customer123",
                    Phone = "0987654321"
                },
                new RegisterDto
                {
                    FullName = "Customer Two",
                    Email = "customer2@example.com",
                    Password = "customer123",
                    Phone = "0912345678"
                }
            };

            foreach (var customer in customers)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync("Auth/register", customer);
                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Failed to register {customer.Email}: {error}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error registering {customer.Email}: {ex.Message}");
                }
            }
        }

        private async Task SeedCategories()
        {
            if (await _context.Categories.AnyAsync())
                return;

            var categories = new[]
            {
                new CreateCategoryDto
                {
                    CategoryName = "Áo Nam",
                    Description = "Áo nam thời trang cao cấp",
                    IsActive = true
                },
                new CreateCategoryDto
                {
                    CategoryName = "Áo Nữ",
                    Description = "Áo nữ thời trang đa dạng",
                    IsActive = true
                },
                new CreateCategoryDto
                {
                    CategoryName = "Quần Nam",
                    Description = "Quần nam phong cách",
                    IsActive = true
                },
                new CreateCategoryDto
                {
                    CategoryName = "Quần Nữ",
                    Description = "Quần nữ thời trang",
                    IsActive = true
                },
                new CreateCategoryDto
                {
                    CategoryName = "Phụ Kiện",
                    Description = "Phụ kiện thời trang",
                    IsActive = true
                }
            };

            foreach (var categoryDto in categories)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync("admin/Categories", categoryDto);
                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Failed to create category {categoryDto.CategoryName}: {error}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating category {categoryDto.CategoryName}: {ex.Message}");
                }
            }
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

            var products = new[]
            {
                // Áo Nam
                new CreateProductDto
                {
                    ProductName = "Áo Sơ Mi Nam Trắng",
                    Price = 350000,
                    Description = "Áo sơ mi nam trắng chất liệu cotton cao cấp, form slim fit",
                    ImageURL = "/images/products/ao-so-mi-nam-trang.jpg",
                    CategoryID = aoNam?.CategoryID ?? 1,
                    Stock = 50,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Áo Thun Nam Basic",
                    Price = 150000,
                    Description = "Áo thun nam basic màu đen, chất liệu cotton 100%",
                    ImageURL = "/images/products/ao-thun-nam-basic.jpg",
                    CategoryID = aoNam?.CategoryID ?? 1,
                    Stock = 100,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Áo Polo Nam",
                    Price = 250000,
                    Description = "Áo polo nam chất liệu cotton pique, thoáng mát",
                    ImageURL = "/images/products/ao-polo-nam.jpg",
                    CategoryID = aoNam?.CategoryID ?? 1,
                    Stock = 75,
                    IsActive = true
                },

                // Áo Nữ
                new CreateProductDto
                {
                    ProductName = "Áo Phông Nữ Dáng Rộng",
                    Price = 180000,
                    Description = "Áo phông nữ dáng rộng màu hồng pastel",
                    ImageURL = "/images/products/ao-phong-nu.jpg",
                    CategoryID = aoNu?.CategoryID ?? 2,
                    Stock = 60,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Áo Sơ Mi Nữ Trắng",
                    Price = 320000,
                    Description = "Áo sơ mi nữ trắng cổ điển, chất liệu chiffon",
                    ImageURL = "/images/products/ao-so-mi-nu.jpg",
                    CategoryID = aoNu?.CategoryID ?? 2,
                    Stock = 45,
                    IsActive = true
                },

                // Quần Nam
                new CreateProductDto
                {
                    ProductName = "Quần Jean Nam Slim",
                    Price = 450000,
                    Description = "Quần jean nam slim fit, màu xanh đậm",
                    ImageURL = "/images/products/quan-jean-nam.jpg",
                    CategoryID = quanNam?.CategoryID ?? 3,
                    Stock = 80,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Quần Kaki Nam",
                    Price = 380000,
                    Description = "Quần kaki nam màu be, form regular",
                    ImageURL = "/images/products/quan-kaki-nam.jpg",
                    CategoryID = quanNam?.CategoryID ?? 3,
                    Stock = 65,
                    IsActive = true
                },

                // Quần Nữ
                new CreateProductDto
                {
                    ProductName = "Quần Jean Nữ Ống Rộng",
                    Price = 420000,
                    Description = "Quần jean nữ ống rộng màu xanh nhạt",
                    ImageURL = "/images/products/quan-jean-nu.jpg",
                    CategoryID = quanNu?.CategoryID ?? 4,
                    Stock = 70,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Quần Short Nữ",
                    Price = 220000,
                    Description = "Quần short nữ màu đen, chất liệu cotton",
                    ImageURL = "/images/products/quan-short-nu.jpg",
                    CategoryID = quanNu?.CategoryID ?? 4,
                    Stock = 55,
                    IsActive = true
                },

                // Phụ Kiện
                new CreateProductDto
                {
                    ProductName = "Túi Xách Da",
                    Price = 550000,
                    Description = "Túi xách da thật màu nâu",
                    ImageURL = "/images/products/tui-xach-da.jpg",
                    CategoryID = phuKien?.CategoryID ?? 5,
                    Stock = 30,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Thắt Lưng Da",
                    Price = 280000,
                    Description = "Thắt lưng da nam màu đen",
                    ImageURL = "/images/products/that-lung-da.jpg",
                    CategoryID = phuKien?.CategoryID ?? 5,
                    Stock = 40,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Mũ Lưỡi Trai",
                    Price = 180000,
                    Description = "Mũ lưỡi trai màu đen",
                    ImageURL = "/images/products/mu-luoi-trai.jpg",
                    CategoryID = phuKien?.CategoryID ?? 5,
                    Stock = 50,
                    IsActive = true
                }
            };

            foreach (var productDto in products)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync("admin/Products", productDto);
                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Failed to create product {productDto.ProductName}: {error}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating product {productDto.ProductName}: {ex.Message}");
                }
            }
        }
    }
}

