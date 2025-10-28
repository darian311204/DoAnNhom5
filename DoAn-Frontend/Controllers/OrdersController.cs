using DoAn_Frontend.Services;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> CreateOrder(string shippingAddress, string phone, string recipientName)
        {
            if (!_apiService.IsAuthenticated()) return Json(new { success = false });
            var order = await _apiService.CreateOrderAsync(shippingAddress, phone, recipientName);
            return Json(new { success = order != null });
        }

        public async Task<IActionResult> History()
        {
            if (!_apiService.IsAuthenticated()) return RedirectToAction("Login", "Auth");
            return View(await _apiService.GetUserOrdersAsync() ?? new List<Models.Order>());
        }
    }
}
