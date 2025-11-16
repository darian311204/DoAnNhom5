using DoAn_Frontend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DoAn_Frontend.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApiService apiService, ILogger<AdminController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public IActionResult Dashboard()
        {
            _logger.LogInformation("Dashboard accessed by user {User}", _apiService.GetCurrentUser()?.Email);
            if (!_apiService.IsAdmin()) return RedirectToAction("Index", "Home");
            return View();
        }

        public async Task<IActionResult> Products()
        {
            _logger.LogInformation("Products list accessed by user {User}", _apiService.GetCurrentUser()?.Email);
            if (!_apiService.IsAdmin()) return RedirectToAction("Index", "Home");
            ViewBag.Categories = await _apiService.GetCategoriesAsync() ?? new List<Models.Category>();
            return View(await _apiService.GetProductsAsync() ?? new List<Models.Product>());
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            _logger.LogInformation("UploadImage called. File: {FileName}, Length: {Length}", file?.FileName, file?.Length);
            if (!_apiService.IsAdmin())
            {
                _logger.LogWarning("UploadImage unauthorized access attempt.");
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("UploadImage received empty file.");
                return BadRequest(new { success = false, message = "No file uploaded" });
            }

            var url = await _apiService.UploadImageAsync(file);
            if (string.IsNullOrEmpty(url))
            {
                _logger.LogError("UploadImage failed in ApiService.UploadImageAsync.");
                return StatusCode(500, new { success = false, message = "Upload failed" });
            }

            _logger.LogInformation("UploadImage succeeded. Url={Url}", url);
            return Json(new { success = true, imageUrl = url });
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Models.Product product)
        {
            if (!_apiService.IsAdmin()) return Json(new { success = false, message = "Unauthorized" });
            
            var success = await _apiService.CreateProductAsync(product);
            return Json(new { success, message = success ? "Product created successfully" : "Failed to create product" });
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            if (!_apiService.IsAdmin()) return Json(new { success = false, message = "Unauthorized" });
            
            Console.WriteLine($"Admin GetProduct called for ID: {id}");
            
            var product = await _apiService.GetProductByIdAsync(id);
            if (product == null) 
            {
                Console.WriteLine($"Product {id} not found");
                return Json(new { success = false, message = "Product not found" });
            }
            
            Console.WriteLine($"Product {id} found: {product.ProductName}");
            return Json(product);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            if (!_apiService.IsAdmin()) return RedirectToAction("Index", "Home");
            
            var product = await _apiService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            
            ViewBag.Categories = await _apiService.GetCategoriesAsync() ?? new List<Models.Category>();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(Models.Product product)
        {
            if (!_apiService.IsAdmin()) return Json(new { success = false, message = "Unauthorized" });
            
            var success = await _apiService.UpdateProductAsync(product);
            return Json(new { success, message = success ? "Product updated successfully" : "Failed to update product" });
        }

        public class DeleteProductRequest
        {
            public int Id { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct([FromBody] DeleteProductRequest request)
        {
            if (!_apiService.IsAdmin())
            {
                _logger.LogWarning("DeleteProduct unauthorized access attempt.");
                return Json(new { success = false, message = "Unauthorized" });
            }

            _logger.LogInformation("DeleteProduct called with Id={Id}", request.Id);
            var success = await _apiService.DeleteProductAsync(request.Id);
            _logger.LogInformation("DeleteProduct result for Id={Id}: {Success}", request.Id, success);
            return Json(new { success, message = success ? "Product deleted successfully" : "Failed to delete product" });
        }

        public async Task<IActionResult> Orders()
        {
            if (!_apiService.IsAdmin()) return RedirectToAction("Index", "Home");
            return View(await _apiService.GetAdminOrdersAsync() ?? new List<Models.Order>());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusRequest request)
        {
            if (!_apiService.IsAdmin())
            {
                _logger.LogWarning("UpdateOrderStatus unauthorized access attempt.");
                return Json(new { success = false, message = "Unauthorized" });
            }

            _logger.LogInformation("UpdateOrderStatus called for OrderId={OrderId}, Status={Status}", request.OrderId, request.Status);
            var success = await _apiService.UpdateOrderStatusAsync(request.OrderId, request.Status, request.CancelReason);
            _logger.LogInformation("UpdateOrderStatus result for OrderId={OrderId}: {Success}", request.OrderId, success);
            return Json(new { success, message = success ? "Order status updated successfully" : "Failed to update order status" });
        }

        public class UpdateOrderStatusRequest
        {
            public int OrderId { get; set; }
            public string Status { get; set; } = string.Empty;
            public string? CancelReason { get; set; }
        }
    }
}
