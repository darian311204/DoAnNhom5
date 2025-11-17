using DoAn_Backend.Services;               // Sử dụng ICartService để xử lý logic giỏ hàng ở tầng service
using Microsoft.AspNetCore.Authorization;  // [Authorize] để yêu cầu JWT
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;              // Dùng để lấy UserId từ JWT
using System.Text.Json.Serialization;

namespace DoAn_Backend.Controllers
{
    [ApiController]                        // Kích hoạt tính năng tự động validate, binding...
    [Route("api/[controller]")]            // => api/cart
    [Authorize]                            // Yêu cầu phải có JWT token khi gọi API
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        // Inject CartService (business logic xử lý giỏ hàng)
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // Hàm này lấy userId từ token JWT
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);    // userId chắc chắn tồn tại vì đã đăng nhập
        }

        // ========================= API: GET giỏ hàng =========================
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userId = GetUserId();                       // Lấy userId từ token
                var cartItems = await _cartService.GetCartItemsAsync(userId); // Lấy danh sách cart
                return Ok(cartItems);                           // Trả về JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ========================= API: ADD giỏ hàng =========================
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] System.Text.Json.JsonElement requestBody)
        {
            try
            {
                // Log JSON nhận được từ frontend
                Console.WriteLine($"Raw request body: {requestBody}");

                // Tự parse JSON bằng tay (để tránh lỗi PascalCase / camelCase)
                int productId = 0;
                int quantity = 1;

                // Lấy ProductId (đầu vào có thể là ProductId hoặc productId)
                if (requestBody.TryGetProperty("ProductId", out var productIdElement))
                {
                    productId = productIdElement.GetInt32();
                }
                else if (requestBody.TryGetProperty("productId", out var productIdElement2))
                {
                    productId = productIdElement2.GetInt32();
                }

                // Lấy Quantity (Quantity / quantity)
                if (requestBody.TryGetProperty("Quantity", out var quantityElement))
                {
                    quantity = quantityElement.GetInt32();
                }
                else if (requestBody.TryGetProperty("quantity", out var quantityElement2))
                {
                    quantity = quantityElement2.GetInt32();
                }

                Console.WriteLine($"Extracted - ProductId: {productId}, Quantity: {quantity}");

                var userId = GetUserId();                    // Lấy userId từ JWT
                Console.WriteLine($"UserId: {userId}");

                // Gọi service để thêm vào giỏ
                var cartItem = await _cartService.AddToCartAsync(userId, productId, quantity);

                // Trả kết quả JSON
                return Ok(new { success = true, cartItem });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding to cart: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ========================= API: UPDATE số lượng =========================
        [HttpPut("{cartId}")]
        public async Task<IActionResult> UpdateCartQuantity(int cartId, [FromBody] int quantity)
        {
            try
            {
                // Service cập nhật số lượng
                var cartItem = await _cartService.UpdateCartQuantityAsync(cartId, quantity);
                return Ok(cartItem);                          // Trả về cart item đã cập nhật
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ========================= API: REMOVE 1 item =========================
        [HttpDelete("{cartId}")]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            try
            {
                var result = await _cartService.RemoveFromCartAsync(cartId);

                if (!result)
                    return NotFound();                       // Cart item không tồn tại

                return Ok(new { message = "Item removed from cart" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ========================= API: CLEAR GIỎ =========================
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetUserId();                    // Lấy userId
                await _cartService.ClearCartAsync(userId);   // Xóa sạch giỏ của user này
                return Ok(new { message = "Cart cleared" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    // DTO chuẩn khi muốn bind JSON kiểu đẹp
    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
