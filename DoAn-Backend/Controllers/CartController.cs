using DoAn_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace DoAn_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userId = GetUserId();
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] System.Text.Json.JsonElement requestBody)
        {
            try
            {
                // Log raw JSON
                Console.WriteLine($"Raw request body: {requestBody}");
                
                // Try to extract productId and quantity manually
                int productId = 0;
                int quantity = 1;
                
                if (requestBody.TryGetProperty("ProductId", out var productIdElement))
                {
                    productId = productIdElement.GetInt32();
                }
                else if (requestBody.TryGetProperty("productId", out var productIdElement2))
                {
                    productId = productIdElement2.GetInt32();
                }
                
                if (requestBody.TryGetProperty("Quantity", out var quantityElement))
                {
                    quantity = quantityElement.GetInt32();
                }
                else if (requestBody.TryGetProperty("quantity", out var quantityElement2))
                {
                    quantity = quantityElement2.GetInt32();
                }
                
                Console.WriteLine($"Extracted - ProductId: {productId}, Quantity: {quantity}");
                
                var userId = GetUserId();
                Console.WriteLine($"UserId: {userId}");
                
                var cartItem = await _cartService.AddToCartAsync(userId, productId, quantity);
                return Ok(new { success = true, cartItem });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding to cart: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{cartId}")]
        public async Task<IActionResult> UpdateCartQuantity(int cartId, [FromBody] int quantity)
        {
            try
            {
                var cartItem = await _cartService.UpdateCartQuantityAsync(cartId, quantity);
                return Ok(cartItem);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{cartId}")]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            try
            {
                var result = await _cartService.RemoveFromCartAsync(cartId);
                if (!result)
                    return NotFound();

                return Ok(new { message = "Item removed from cart" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetUserId();
                await _cartService.ClearCartAsync(userId);
                return Ok(new { message = "Cart cleared" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
