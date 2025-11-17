using DoAn_Backend.Data;                 // Chứa ApplicationDbContext (EF Core)
using DoAn_Backend.Models;               // Chứa entity Cart, Product, Category...
using Microsoft.EntityFrameworkCore;      // Dùng ToListAsync, Include, FindAsync...

namespace DoAn_Backend.Services
{
    // CartService triển khai ICartService => service chứa logic giỏ hàng
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        // Inject DbContext
        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==============================
        // Lấy toàn bộ cart items của user
        // ==============================
        public async Task<IEnumerable<Cart>> GetCartItemsAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Product)                   // Include Product để FE có thông tin sản phẩm
                .ThenInclude(p => p!.Category)             // Include luôn Category
                .Where(c => c.UserID == userId)            // Chỉ lấy cart của user
                .ToListAsync();
        }

        // ===================================
        // Lấy 1 cart item dựa theo user + product
        // (dùng để kiểm tra xem sản phẩm đã có trong giỏ chưa)
        // ===================================
        public async Task<Cart?> GetCartItemAsync(int userId, int productId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c =>
                    c.UserID == userId && c.ProductID == productId);
        }

        // ==============================
        // Thêm sản phẩm vào giỏ
        // ==============================
        public async Task<Cart> AddToCartAsync(int userId, int productId, int quantity)
        {
            // Kiểm tra sản phẩm có tồn tại không
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
            if (product == null)
            {
                // Log để debug khi dùng sai ProductId
                var allProductIds = await _context.Products.Select(p => p.ProductID).ToListAsync();
                Console.WriteLine($"Product {productId} not found. Available product IDs: {string.Join(", ", allProductIds)}");

                throw new Exception($"Product with ID {productId} does not exist");
            }

            // Kiểm tra sản phẩm có bị ẩn (IsActive = false) không
            if (!product.IsActive)
            {
                throw new Exception($"Product with ID {productId} is not active");
            }

            // Kiểm tra xem user đã có sản phẩm này trong giỏ chưa
            var existingCart = await GetCartItemAsync(userId, productId);

            if (existingCart != null)
            {
                // Nếu có rồi → tăng số lượng
                existingCart.Quantity += quantity;
                _context.Carts.Update(existingCart);
            }
            else
            {
                // Nếu chưa có → tạo cart item mới
                existingCart = new Cart
                {
                    UserID = userId,
                    ProductID = productId,
                    Quantity = quantity,
                    AddedAt = DateTime.Now
                };

                _context.Carts.Add(existingCart);
            }

            // Lưu thay đổi vào DB
            await _context.SaveChangesAsync();

            return existingCart;            // Trả về cart item sau khi thêm/cập nhật
        }

        // ==============================
        // Cập nhật số lượng cart item
        // ==============================
        public async Task<Cart> UpdateCartQuantityAsync(int cartId, int quantity)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);

            if (cartItem == null)
                throw new Exception("Cart item not found");  // Cart item không tồn tại

            // Gán số lượng mới
            cartItem.Quantity = quantity;
            _context.Carts.Update(cartItem);

            await _context.SaveChangesAsync();
            return cartItem;
        }

        // ==============================
        // Xóa 1 item khỏi giỏ
        // ==============================
        public async Task<bool> RemoveFromCartAsync(int cartId)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem == null)
                return false;                // Không tồn tại

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();

            return true;
        }

        // ==============================
        // Xóa toàn bộ giỏ hàng của user
        // ==============================
        public async Task<bool> ClearCartAsync(int userId)
        {
            // Lấy toàn bộ các cart item của user
            var cartItems = await _context.Carts
                .Where(c => c.UserID == userId)
                .ToListAsync();

            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
