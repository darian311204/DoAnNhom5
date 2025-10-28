using DoAn_Backend.Data;
using DoAn_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn_Backend.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cart>> GetCartItemsAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .ThenInclude(p => p!.Category)
                .Where(c => c.UserID == userId)
                .ToListAsync();
        }

        public async Task<Cart?> GetCartItemAsync(int userId, int productId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.UserID == userId && c.ProductID == productId);
        }

        public async Task<Cart> AddToCartAsync(int userId, int productId, int quantity)
        {
            // Verify that the product exists
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
            if (product == null)
            {
                // Get all product IDs for debugging
                var allProductIds = await _context.Products.Select(p => p.ProductID).ToListAsync();
                Console.WriteLine($"Product {productId} not found. Available product IDs: {string.Join(", ", allProductIds)}");
                throw new Exception($"Product with ID {productId} does not exist");
            }
            
            if (!product.IsActive)
            {
                throw new Exception($"Product with ID {productId} is not active");
            }

            var existingCart = await GetCartItemAsync(userId, productId);
            
            if (existingCart != null)
            {
                existingCart.Quantity += quantity;
                _context.Carts.Update(existingCart);
            }
            else
            {
                existingCart = new Cart
                {
                    UserID = userId,
                    ProductID = productId,
                    Quantity = quantity,
                    AddedAt = DateTime.Now
                };
                _context.Carts.Add(existingCart);
            }

            await _context.SaveChangesAsync();
            return existingCart;
        }

        public async Task<Cart> UpdateCartQuantityAsync(int cartId, int quantity)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem == null)
                throw new Exception("Cart item not found");

            cartItem.Quantity = quantity;
            _context.Carts.Update(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<bool> RemoveFromCartAsync(int cartId)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem == null)
                return false;

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cartItems = await _context.Carts.Where(c => c.UserID == userId).ToListAsync();
            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
