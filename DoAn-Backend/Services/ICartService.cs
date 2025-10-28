using DoAn_Backend.Models;

namespace DoAn_Backend.Services
{
    public interface ICartService
    {
        Task<IEnumerable<Cart>> GetCartItemsAsync(int userId);
        Task<Cart?> GetCartItemAsync(int userId, int productId);
        Task<Cart> AddToCartAsync(int userId, int productId, int quantity);
        Task<Cart> UpdateCartQuantityAsync(int cartId, int quantity);
        Task<bool> RemoveFromCartAsync(int cartId);
        Task<bool> ClearCartAsync(int userId);
    }
}
