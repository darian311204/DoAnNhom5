using DoAn_Frontend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace DoAn_Frontend.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApiService _apiService;

        public OrdersController(ApiService apiService) => _apiService = apiService;

        public async Task<IActionResult> Checkout()
        {
            if (!_apiService.IsAuthenticated()) return RedirectToAction("Login", "Auth");
            var cart = await _apiService.GetCartAsync();
            if (cart == null || !cart.Any()) return RedirectToAction("Index", "Cart");
            ViewBag.Cart = cart;
            ViewBag.User = _apiService.GetCurrentUser();
            return View();
        }

        public class CreateOrderRequest
        {
            [JsonPropertyName("shippingAddress")]
            public string ShippingAddress { get; set; } = string.Empty;
            
            [JsonPropertyName("phone")]
            public string Phone { get; set; } = string.Empty;
            
            [JsonPropertyName("recipientName")]
            public string RecipientName { get; set; } = string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!_apiService.IsAuthenticated()) return Json(new { success = false, message = "Unauthorized" });
            
            if (request == null || string.IsNullOrWhiteSpace(request.ShippingAddress) || 
                string.IsNullOrWhiteSpace(request.Phone) || string.IsNullOrWhiteSpace(request.RecipientName))
            {
                return Json(new { success = false, message = "All fields are required" });
            }
            
            try
            {
                var order = await _apiService.CreateOrderAsync(request.ShippingAddress, request.Phone, request.RecipientName);
                return Json(new { success = order != null });
            }
            catch (Exception ex)
            {
                // Surface backend error message to the frontend for debugging
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> History()
        {
            if (!_apiService.IsAuthenticated()) return RedirectToAction("Login", "Auth");
            return View(await _apiService.GetUserOrdersAsync() ?? new List<Models.Order>());
        }
    }
}
