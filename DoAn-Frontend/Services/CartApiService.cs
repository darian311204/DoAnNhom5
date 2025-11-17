using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    // CartApiService kế thừa BaseApiService và implement ICartApiService
    // => Service này dùng để gọi API giỏ hàng của backend
    public class CartApiService : BaseApiService, ICartApiService
    {
        // Constructor: nhận HttpClient, cấu hình, HttpContextAccessor để lấy token
        public CartApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, configuration, httpContextAccessor)  // gọi constructor của BaseApiService
        {
        }

        // Lấy danh sách giỏ hàng của người dùng hiện tại
        public async Task<List<Cart>?> GetCartAsync()
        {
            // Tạo request GET /Cart (đã tự gắn token trong BaseApiService)
            var request = CreateRequest(HttpMethod.Get, "Cart");

            // Gửi request đến API backend
            var response = await _httpClient.SendAsync(request);

            // Nếu thành công → đọc JSON trả về thành List<Cart>
            // Nếu lỗi → trả về danh sách rỗng
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<Cart>>() : new List<Cart>();
        }

        // Thêm một sản phẩm vào giỏ hàng
        public async Task<bool> AddToCartAsync(int productId, int quantity)
        {
            try
            {
                // Tạo nội dung gửi lên API dạng JSON (PascalCase cho đúng backend)
                var content = JsonContent.Create(new { ProductId = productId, Quantity = quantity });

                // In ra console để debug
                Console.WriteLine($"Sending to cart: ProductId={productId}, Quantity={quantity}");

                // Tạo request POST /Cart/add kèm body JSON
                var request = CreateRequest(HttpMethod.Post, "Cart/add", content);

                // Gửi request
                var response = await _httpClient.SendAsync(request);

                // Nếu thêm thành công → return true
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    // Nếu lỗi → đọc nội dung lỗi để debug
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Add to cart failed: {response.StatusCode} - {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Nếu exception → report lỗi
                Console.WriteLine($"Add to cart error: {ex.Message}");
                return false;
            }
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        public async Task<bool> UpdateCartQuantityAsync(int cartId, int quantity)
        {
            try
            {
                // Tạo nội dung JSON đơn giản chứa quantity
                var content = new StringContent(quantity.ToString(), System.Text.Encoding.UTF8, "application/json");

                // Tạo request PUT /Cart/{cartId}
                var request = CreateRequest(HttpMethod.Put, $"Cart/{cartId}", content);

                // Gửi request
                var response = await _httpClient.SendAsync(request);

                // Thành công → true
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update cart error: {ex.Message}");
                return false;
            }
        }

        // Xóa một sản phẩm ra khỏi giỏ hàng
        public async Task<bool> RemoveFromCartAsync(int cartId)
        {
            // Tạo request DELETE /Cart/{cartId}
            var request = CreateRequest(HttpMethod.Delete, $"Cart/{cartId}");

            // Gửi request
            var response = await _httpClient.SendAsync(request);

            // Trả về true nếu thành công
            return response.IsSuccessStatusCode;
        }

        // Lấy tổng số lượng item có trong giỏ
        public async Task<int> GetCartItemCountAsync()
        {
            try
            {
                // Gọi lại hàm GetCartAsync
                var cart = await GetCartAsync();

                // Tính tổng quantity tất cả item
                return cart?.Sum(item => item.Quantity) ?? 0;
            }
            catch
            {
                // Nếu lỗi → trả về 0
                return 0;
            }
        }
    }
}
