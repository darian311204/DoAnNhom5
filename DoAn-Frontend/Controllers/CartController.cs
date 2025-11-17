using DoAn_Frontend.Services;              // Sử dụng ApiService để gọi API backend
using Microsoft.AspNetCore.Mvc;            // Dùng cho Controller, IActionResult,...

namespace DoAn_Frontend.Controllers
{
    public class CartController : Controller
    {
        private readonly ApiService _apiService;    // Khai báo service gọi API

        // Inject ApiService qua constructor
        public CartController(ApiService apiService) => _apiService = apiService;

        // Trang hiển thị giỏ hàng
        public async Task<IActionResult> Index()
        {
            // Nếu chưa đăng nhập thì chuyển sang trang Login
            if (!_apiService.IsAuthenticated()) return RedirectToAction("Login", "Auth");

            // Gọi API lấy danh sách cart, nếu null thì trả về List rỗng
            return View(await _apiService.GetCartAsync() ?? new List<Models.Cart>());
        }

        // Lấy số lượng sản phẩm trong giỏ (được gọi bằng AJAX)
        [HttpGet]
        public async Task<IActionResult> GetCount()
        {
            // Chưa login => trả về 0
            if (!_apiService.IsAuthenticated())
                return Content("0");

            // Gọi API lấy số lượng cart items
            var count = await _apiService.GetCartItemCountAsync();

            // Trả về text/plain
            return Content(count.ToString());
        }

        // API thêm sản phẩm vào giỏ
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] System.Text.Json.JsonElement request)
        {
            // Không đăng nhập => fail
            if (!_apiService.IsAuthenticated())
                return Json(new { success = false, message = "Please login" });

            // Khai báo biến để chứa productId và quantity
            int productId = 0;
            int quantity = 1;

            try
            {
                // Thử lấy "productId" (camelCase)
                if (request.TryGetProperty("productId", out var productIdElement))
                {
                    productId = productIdElement.GetInt32();
                }
                // Hoặc "ProductId" (PascalCase)
                else if (request.TryGetProperty("ProductId", out var productIdElement2))
                {
                    productId = productIdElement2.GetInt32();
                }

                // Lấy quantity (camelCase)
                if (request.TryGetProperty("quantity", out var quantityElement))
                {
                    quantity = quantityElement.GetInt32();
                }
                // Hoặc PascalCase
                else if (request.TryGetProperty("Quantity", out var quantityElement2))
                {
                    quantity = quantityElement2.GetInt32();
                }

                // Log: phục vụ debug frontend gửi gì
                Console.WriteLine($"Frontend Cart.Add received - ProductId: {productId}, Quantity: {quantity}");
            }
            catch (Exception ex)
            {
                // Parse JSON lỗi
                Console.WriteLine($"Error parsing request: {ex.Message}");
                return Json(new { success = false, message = "Invalid request format" });
            }

            // Gọi API backend để thêm vào giỏ
            var success = await _apiService.AddToCartAsync(productId, quantity);

            // Trả về JSON cho AJAX
            return Json(new { success, message = success ? "Added" : "Failed" });
        }

        // Cập nhật số lượng sp trong giỏ
        [HttpPost]
        public async Task<IActionResult> Update(int cartId, int quantity)
        {
            // Chưa đăng nhập => báo lỗi
            if (!_apiService.IsAuthenticated())
                return Json(new { success = false, message = "Please login" });

            // Gọi API backend để update số lượng
            var success = await _apiService.UpdateCartQuantityAsync(cartId, quantity);

            return Json(new { success, message = success ? "Updated" : "Failed" });
        }

        // Xóa 1 sản phẩm khỏi giỏ
        [HttpPost]
        public async Task<IActionResult> Remove(int cartId)
        {
            // Không login => unauthorized
            if (!_apiService.IsAuthenticated())
                return Json(new { success = false, message = "Unauthorized" });

            // Gọi API xóa item
            var success = await _apiService.RemoveFromCartAsync(cartId);

            // Nếu là AJAX => trả JSON
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success, message = success ? "Removed" : "Failed" });
            }

            // Nếu request thường => redirect về trang giỏ hàng
            return RedirectToAction("Index");
        }
    }
}
