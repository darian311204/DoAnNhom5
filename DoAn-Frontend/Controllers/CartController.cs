using DoAn_Frontend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoAn_Frontend.Controllers
{
    public class CartController : Controller
    {
        private readonly ApiService _apiService;

        public CartController(ApiService apiService) => _apiService = apiService;

        public async Task<IActionResult> Index()
        {
            if (!_apiService.IsAuthenticated()) return RedirectToAction("Login", "Auth");
            return View(await _apiService.GetCartAsync() ?? new List<Models.Cart>());
        }

        [HttpGet]
        public async Task<IActionResult> GetCount()
        {
            if (!_apiService.IsAuthenticated()) 
                return Content("0");
            
            var count = await _apiService.GetCartItemCountAsync();
            return Content(count.ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] System.Text.Json.JsonElement request)
        {
            if (!_apiService.IsAuthenticated()) return Json(new { success = false, message = "Please login" });
            
            // Extract productId and quantity from JsonElement
            int productId = 0;
            int quantity = 1;
            
            try 
            {
                // Try both camelCase and PascalCase
                if (request.TryGetProperty("productId", out var productIdElement))
                {
                    productId = productIdElement.GetInt32();
                }
                else if (request.TryGetProperty("ProductId", out var productIdElement2))
                {
                    productId = productIdElement2.GetInt32();
                }
                
                if (request.TryGetProperty("quantity", out var quantityElement))
                {
                    quantity = quantityElement.GetInt32();
                }
                else if (request.TryGetProperty("Quantity", out var quantityElement2))
                {
                    quantity = quantityElement2.GetInt32();
                }
                
                Console.WriteLine($"Frontend Cart.Add received - ProductId: {productId}, Quantity: {quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing request: {ex.Message}");
                return Json(new { success = false, message = "Invalid request format" });
            }
            
            var success = await _apiService.AddToCartAsync(productId, quantity);
            return Json(new { success, message = success ? "Added" : "Failed" });
        }

        [HttpPost]
        public async Task<IActionResult> Update(int cartId, int quantity)
        {
            if (!_apiService.IsAuthenticated()) 
                return Json(new { success = false, message = "Please login" });
            
            var success = await _apiService.UpdateCartQuantityAsync(cartId, quantity);
            return Json(new { success, message = success ? "Updated" : "Failed" });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int cartId)
        {
            if (!_apiService.IsAuthenticated()) return Json(new { success = false, message = "Unauthorized" });
            
            var success = await _apiService.RemoveFromCartAsync(cartId);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success, message = success ? "Removed" : "Failed" });
            }
            
            return RedirectToAction("Index");
        }
    }
}
