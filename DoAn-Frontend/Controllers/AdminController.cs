using DoAn_Frontend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoAn_Frontend.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApiService _apiService;

        public AdminController(ApiService apiService) => _apiService = apiService;

        public IActionResult Dashboard()
        {
            if (!_apiService.IsAdmin()) return RedirectToAction("Index", "Home");
            return View();
        }

        public async Task<IActionResult> Products()
        {
            if (!_apiService.IsAdmin()) return RedirectToAction("Index", "Home");
            ViewBag.Categories = await _apiService.GetCategoriesAsync() ?? new List<Models.Category>();
            return View(await _apiService.GetProductsAsync() ?? new List<Models.Product>());
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

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!_apiService.IsAdmin()) return Json(new { success = false, message = "Unauthorized" });
            
            var success = await _apiService.DeleteProductAsync(id);
            return Json(new { success, message = success ? "Product deleted successfully" : "Failed to delete product" });
        }

        public async Task<IActionResult> Orders()
        {
            if (!_apiService.IsAdmin()) return RedirectToAction("Index", "Home");
            return View(await _apiService.GetAdminOrdersAsync() ?? new List<Models.Order>());
        }
    }
}
