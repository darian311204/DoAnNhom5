using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    /// <summary>
    /// Composite service that delegates to specialized API services.
    /// Maintained for backward compatibility with existing controllers and views.
    /// </summary>
    public class ApiService
    {
        private readonly IAuthApiService _authService;
        private readonly IProductApiService _productService;
        private readonly ICartApiService _cartService;
        private readonly IOrderApiService _orderService;
        private readonly IReviewApiService _reviewService;
        private readonly IAdminApiService _adminService;

        public ApiService(
            IAuthApiService authService,
            IProductApiService productService,
            ICartApiService cartService,
            IOrderApiService orderService,
            IReviewApiService reviewService,
            IAdminApiService adminService)
        {
            _authService = authService;
            _productService = productService;
            _cartService = cartService;
            _orderService = orderService;
            _reviewService = reviewService;
            _adminService = adminService;
        }

        // Auth delegation
        public bool IsTokenExpired() => _authService.IsTokenExpired();
        public async Task<AuthResponse?> LoginAsync(LoginDto dto) => await _authService.LoginAsync(dto);
        public async Task<AuthResponse?> RegisterAsync(RegisterDto dto) => await _authService.RegisterAsync(dto);
        public void Logout() => _authService.Logout();
        public bool IsAuthenticated() => _authService.IsAuthenticated();
        public bool IsAdmin() => _authService.IsAdmin();
        public User? GetCurrentUser() => _authService.GetCurrentUser();

        // Product delegation
        public async Task<List<Product>?> GetProductsAsync() => await _productService.GetProductsAsync();
        public async Task<Product?> GetProductAsync(int id) => await _productService.GetProductAsync(id);
        public async Task<List<Product>?> SearchProductsAsync(string searchTerm) => await _productService.SearchProductsAsync(searchTerm);
        public async Task<List<Category>?> GetCategoriesAsync() => await _productService.GetCategoriesAsync();

        // Cart delegation
        public async Task<List<Cart>?> GetCartAsync() => await _cartService.GetCartAsync();
        public async Task<bool> AddToCartAsync(int productId, int quantity) => await _cartService.AddToCartAsync(productId, quantity);
        public async Task<bool> UpdateCartQuantityAsync(int cartId, int quantity) => await _cartService.UpdateCartQuantityAsync(cartId, quantity);
        public async Task<bool> RemoveFromCartAsync(int cartId) => await _cartService.RemoveFromCartAsync(cartId);
        public async Task<int> GetCartItemCountAsync() => await _cartService.GetCartItemCountAsync();

        // Order delegation
        public async Task<Order?> CreateOrderAsync(string shippingAddress, string phone, string recipientName)
            => await _orderService.CreateOrderAsync(shippingAddress, phone, recipientName);
        public async Task<List<Order>?> GetUserOrdersAsync() => await _orderService.GetUserOrdersAsync();

        // Review delegation
        public async Task<List<Review>?> GetProductReviewsAsync(int productId) => await _reviewService.GetProductReviewsAsync(productId);
        public async Task<bool> AddReviewAsync(int productId, int rating, string comment)
            => await _reviewService.AddReviewAsync(productId, rating, comment);

        // Admin delegation
        public async Task<List<Order>?> GetAdminOrdersAsync() => await _adminService.GetAdminOrdersAsync();
        public async Task<bool> CreateProductAsync(Product product) => await _adminService.CreateProductAsync(product);
        public async Task<Product?> GetProductByIdAsync(int id) => await _adminService.GetProductByIdAsync(id);
        public async Task<bool> UpdateProductAsync(Product product) => await _adminService.UpdateProductAsync(product);
        public async Task<bool> DeleteProductAsync(int productId) => await _adminService.DeleteProductAsync(productId);
        public async Task<string?> UploadImageAsync(IFormFile file) => await _adminService.UploadImageAsync(file);
    }
}
