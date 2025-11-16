using DoAn_Backend.DTOs;
using DoAn_Backend.Models;
using DoAn_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DoAn_Backend.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IAdminService adminService, ILogger<ProductsController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _adminService.CreateProductAsync(dto);
                _logger.LogInformation("CreateProduct - created ProductID={Id}, Name={Name}", result.ProductID, result.ProductName);
                return CreatedAtAction(nameof(GetProduct), new { id = result.ProductID }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateProduct failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(
            IFormFile file,
            [FromServices] IWebHostEnvironment env,
            [FromServices] IConfiguration configuration)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("UploadImage called with empty file.");
                    return BadRequest(new { message = "No file uploaded" });
                }

                var contentRoot = env.WebRootPath ?? System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot");
                var destDir = System.IO.Path.Combine(contentRoot, "images", "products");
                System.IO.Directory.CreateDirectory(destDir);

                var fileName = $"{Guid.NewGuid()}{System.IO.Path.GetExtension(file.FileName)}";
                var destPath = System.IO.Path.Combine(destDir, fileName);

                using (var stream = new System.IO.FileStream(destPath, System.IO.FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var apiBase = configuration["ApiBaseUrl"] ?? "http://localhost:5090/api/";
                apiBase = apiBase.TrimEnd('/');
                if (apiBase.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
                    apiBase = apiBase[..^4];

                var imageUrl = $"{apiBase}/images/products/{fileName}";
                _logger.LogInformation("UploadImage succeeded. Url={Url}", imageUrl);
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UploadImage failed");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _adminService.UpdateProductAsync(id, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _adminService.DeleteProductAsync(id);
                if (!result)
                    return NotFound();

                return Ok(new { message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                // This would typically use a separate service, but for now using the product service
                var productService = HttpContext.RequestServices.GetService<IProductService>();
                if (productService == null)
                    return StatusCode(500, new { message = "Service not available" });

                var product = await productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound();

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
