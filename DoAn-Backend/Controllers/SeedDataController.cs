using DoAn_Backend.Data;
using DoAn_Backend.DTOs;
using DoAn_Backend.Models;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _env;
        private string? _adminToken;

        public SeedDataController(
            ApplicationDbContext context,
            HttpClient httpClient,
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
            _env = env;

            // Set base address for API calls
            var baseUrl = _configuration["ApiBaseUrl"] ?? "http://localhost:5090/api/";
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }
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

                // Always start from a clean state when seeding:
                // remove dependent entities first to satisfy FK constraints
                _context.OrderDetails.RemoveRange(_context.OrderDetails);
                _context.Orders.RemoveRange(_context.Orders);
                _context.Reviews.RemoveRange(_context.Reviews);
                _context.Carts.RemoveRange(_context.Carts);
                _context.Products.RemoveRange(_context.Products);
                _context.Categories.RemoveRange(_context.Categories);
                _context.Users.RemoveRange(_context.Users);
                await _context.SaveChangesAsync();

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
                // Prepare login payload
                var loginDto = new LoginDto
                {
                    Email = "admin@example.com",
                    Password = "admin123"
                };

                // First, try to login assuming admin already exists and has Admin role
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
                    // Update user role to Admin directly in DB (since register creates Customer)
                    var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "admin@example.com");
                    if (adminUser != null)
                    {
                        adminUser.Role = "Admin";
                        await _context.SaveChangesAsync();
                    }

                    // Now login again to get a token that includes the Admin role
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
                },
                new RegisterDto
                {
                    FullName = "Nguyễn Văn A",
                    Email = "nguyenvana@example.com",
                    Password = "customer123",
                    Phone = "0901234567"
                },
                new RegisterDto
                {
                    FullName = "Trần Thị B",
                    Email = "tranthib@example.com",
                    Password = "customer123",
                    Phone = "0902345678"
                },
                new RegisterDto
                {
                    FullName = "Lê Văn C",
                    Email = "levanc@example.com",
                    Password = "customer123",
                    Phone = "0903456789"
                },
                new RegisterDto
                {
                    FullName = "Phạm Thị D",
                    Email = "phamthid@example.com",
                    Password = "customer123",
                    Phone = "0904567890"
                },
                new RegisterDto
                {
                    FullName = "Hoàng Văn E",
                    Email = "hoangvane@example.com",
                    Password = "customer123",
                    Phone = "0905678901"
                },
                new RegisterDto
                {
                    FullName = "Vũ Thị F",
                    Email = "vuthif@example.com",
                    Password = "customer123",
                    Phone = "0906789012"
                },
                new RegisterDto
                {
                    FullName = "Đỗ Văn G",
                    Email = "dovang@example.com",
                    Password = "customer123",
                    Phone = "0907890123"
                },
                new RegisterDto
                {
                    FullName = "Bùi Thị H",
                    Email = "buithih@example.com",
                    Password = "customer123",
                    Phone = "0908901234"
                },
                new RegisterDto
                {
                    FullName = "Đinh Văn I",
                    Email = "dinhvani@example.com",
                    Password = "customer123",
                    Phone = "0909012345"
                },
                new RegisterDto
                {
                    FullName = "Mai Thị K",
                    Email = "maithik@example.com",
                    Password = "customer123",
                    Phone = "0910123456"
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
                },
                new CreateCategoryDto
                {
                    CategoryName = "Áo Khoác",
                    Description = "Áo khoác nam nữ đa dạng kiểu dáng",
                    IsActive = true
                },
                new CreateCategoryDto
                {
                    CategoryName = "Giày Dép",
                    Description = "Giày dép thời trang cao cấp",
                    IsActive = true
                },
                new CreateCategoryDto
                {
                    CategoryName = "Váy Đầm",
                    Description = "Váy đầm nữ sang trọng",
                    IsActive = true
                },
                new CreateCategoryDto
                {
                    CategoryName = "Đồ Thể Thao",
                    Description = "Trang phục thể thao năng động",
                    IsActive = true
                },
                new CreateCategoryDto
                {
                    CategoryName = "Đồ Ngủ",
                    Description = "Đồ ngủ thoải mái cho nam nữ",
                    IsActive = true
                },
                new CreateCategoryDto
                {
                    CategoryName = "Đồ Lót",
                    Description = "Đồ lót cao cấp",
                    IsActive = true
                },
                new CreateCategoryDto
                {
                    CategoryName = "Túi Xách",
                    Description = "Túi xách thời trang đa dạng mẫu mã",
                    IsActive = true
                },
                new CreateCategoryDto
                {
                    CategoryName = "Đồng Hồ",
                    Description = "Đồng hồ nam nữ chính hãng",
                    IsActive = true
                },
                new CreateCategoryDto
                {
                    CategoryName = "Trang Sức",
                    Description = "Trang sức thời trang cao cấp",
                    IsActive = true
                },
                new CreateCategoryDto
                {
                    CategoryName = "Nước Hoa",
                    Description = "Nước hoa chính hãng nhiều mùi hương",
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

        private string GetImageUrlFromResources(string fileName)
        {
            try
            {
                // Determine source (Resources) and destination (wwwroot/images/products) paths
                var contentRoot = _env.ContentRootPath ?? Directory.GetCurrentDirectory();
                var resourcesDir = Path.Combine(contentRoot, "Resources");
                var webRoot = _env.WebRootPath ?? Path.Combine(contentRoot, "wwwroot");
                var destDir = Path.Combine(webRoot, "images", "products");

                Directory.CreateDirectory(destDir);

                var sourcePath = Path.Combine(resourcesDir, fileName);
                var destPath = Path.Combine(destDir, fileName);

                if (System.IO.File.Exists(sourcePath))
                {
                    if (!System.IO.File.Exists(destPath))
                    {
                        System.IO.File.Copy(sourcePath, destPath, overwrite: false);
                    }

                    // Build absolute URL to backend so frontend can load images from API host
                    var apiBase = _configuration["ApiBaseUrl"] ?? "http://localhost:5090/api/";
                    // Strip trailing slash
                    if (apiBase.EndsWith("/"))
                        apiBase = apiBase.TrimEnd('/');
                    // Strip "/api" suffix if present to get backend root
                    if (apiBase.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
                        apiBase = apiBase[..^4];

                    return $"{apiBase}/images/products/{fileName}";
                }

                Console.WriteLine($"SeedData warning: image file not found in Resources: {sourcePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SeedData error copying image '{fileName}': {ex.Message}");
            }

            // Fallback (frontend will show placeholder on error)
            return "https://via.placeholder.com/300x300";
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
                    ImageURL = GetImageUrlFromResources("ao-somi-nam-1.jpg"),
                    CategoryID = aoNam?.CategoryID ?? 1,
                    Stock = 50,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Áo Thun Nam Basic",
                    Price = 150000,
                    Description = "Áo thun nam basic màu đen, chất liệu cotton 100%",
                    ImageURL = GetImageUrlFromResources("aophong-den-nam.png"),
                    CategoryID = aoNam?.CategoryID ?? 1,
                    Stock = 100,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Áo Polo Nam",
                    Price = 250000,
                    Description = "Áo polo nam chất liệu cotton pique, thoáng mát",
                    ImageURL = GetImageUrlFromResources("ao-polo-nam.jpg"),
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
                    ImageURL = GetImageUrlFromResources("ao-thun-nu-1.jpg"),
                    CategoryID = aoNu?.CategoryID ?? 2,
                    Stock = 60,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Áo Sơ Mi Nữ Trắng",
                    Price = 320000,
                    Description = "Áo sơ mi nữ trắng cổ điển, chất liệu chiffon",
                    ImageURL = GetImageUrlFromResources("ao-thun-nu-2.jpg"),
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
                    ImageURL = GetImageUrlFromResources("quan-jean-1.jpg"),
                    CategoryID = quanNam?.CategoryID ?? 3,
                    Stock = 80,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Quần Kaki Nam",
                    Price = 380000,
                    Description = "Quần kaki nam màu be, form regular",
                    ImageURL = GetImageUrlFromResources("quan-somi-1.jpg"),
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
                    ImageURL = GetImageUrlFromResources("quan-somi-2.jpg"),
                    CategoryID = quanNu?.CategoryID ?? 4,
                    Stock = 70,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Quần Short Nữ",
                    Price = 220000,
                    Description = "Quần short nữ màu đen, chất liệu cotton",
                    ImageURL = GetImageUrlFromResources("quan-short-nam-1.jpg"),
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
                    ImageURL = GetImageUrlFromResources("ao-khoac-nu.jpg"),
                    CategoryID = phuKien?.CategoryID ?? 5,
                    Stock = 30,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Thắt Lưng Da",
                    Price = 280000,
                    Description = "Thắt lưng da nam màu đen",
                    ImageURL = GetImageUrlFromResources("ao-khoac-kaki-nam.jpg"),
                    CategoryID = phuKien?.CategoryID ?? 5,
                    Stock = 40,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Mũ Lưỡi Trai",
                    Price = 180000,
                    Description = "Mũ lưỡi trai màu đen",
                    ImageURL = GetImageUrlFromResources("ao-phao-nam.jpg"),
                    CategoryID = phuKien?.CategoryID ?? 5,
                    Stock = 50,
                    IsActive = true
                },
                // Thêm sản phẩm mới
                new CreateProductDto
                {
                    ProductName = "Áo Khoác Jean Nam",
                    Price = 580000,
                    Description = "Áo khoác jean nam màu xanh đậm, phong cách năng động",
                    ImageURL = GetImageUrlFromResources("ao-khoac-jean-nam.jpg"),
                    CategoryID = aoNam?.CategoryID ?? 1,
                    Stock = 35,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Áo Len Nam",
                    Price = 420000,
                    Description = "Áo len nam cổ tròn, chất liệu len cao cấp",
                    ImageURL = GetImageUrlFromResources("ao-len-nam.jpg"),
                    CategoryID = aoNam?.CategoryID ?? 1,
                    Stock = 40,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Áo Blazer Nữ",
                    Price = 680000,
                    Description = "Áo blazer nữ công sở màu xám",
                    ImageURL = GetImageUrlFromResources("ao-blazer-nu.jpg"),
                    CategoryID = aoNu?.CategoryID ?? 2,
                    Stock = 25,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Áo Cardigan Nữ",
                    Price = 380000,
                    Description = "Áo cardigan nữ màu kem, dáng dài",
                    ImageURL = GetImageUrlFromResources("ao-cardigan-nu.jpg"),
                    CategoryID = aoNu?.CategoryID ?? 2,
                    Stock = 50,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Quần Jogger Nam",
                    Price = 320000,
                    Description = "Quần jogger nam thể thao, chất liệu thun co giãn",
                    ImageURL = GetImageUrlFromResources("quan-jogger-nam.jpg"),
                    CategoryID = quanNam?.CategoryID ?? 3,
                    Stock = 60,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Quần Âu Nam",
                    Price = 480000,
                    Description = "Quần âu nam công sở màu đen",
                    ImageURL = GetImageUrlFromResources("quan-au-nam.jpg"),
                    CategoryID = quanNam?.CategoryID ?? 3,
                    Stock = 45,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Váy Nữ Dáng Xòe",
                    Price = 390000,
                    Description = "Váy nữ dáng xòe màu hồng nhạt",
                    ImageURL = GetImageUrlFromResources("vay-nu-xoe.jpg"),
                    CategoryID = quanNu?.CategoryID ?? 4,
                    Stock = 35,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Quần Culottes Nữ",
                    Price = 350000,
                    Description = "Quần culottes nữ màu trắng, chất vải lụa",
                    ImageURL = GetImageUrlFromResources("quan-culottes-nu.jpg"),
                    CategoryID = quanNu?.CategoryID ?? 4,
                    Stock = 42,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Kính Mát Nam Nữ",
                    Price = 320000,
                    Description = "Kính mát thời trang chống UV",
                    ImageURL = GetImageUrlFromResources("kinh-mat.jpg"),
                    CategoryID = phuKien?.CategoryID ?? 5,
                    Stock = 55,
                    IsActive = true
                },
                new CreateProductDto
                {
                    ProductName = "Ví Da Nam",
                    Price = 380000,
                    Description = "Ví da nam cao cấp màu nâu đen",
                    ImageURL = GetImageUrlFromResources("vi-da-nam.jpg"),
                    CategoryID = phuKien?.CategoryID ?? 5,
                    Stock = 48,
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

