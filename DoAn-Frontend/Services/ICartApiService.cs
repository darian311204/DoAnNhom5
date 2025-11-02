using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    public interface ICartApiService
    {
        Task<List<Cart>?> GetCartAsync();
        Task<bool> AddToCartAsync(int productId, int quantity);
        Task<bool> UpdateCartQuantityAsync(int cartId, int quantity);
        Task<bool> RemoveFromCartAsync(int cartId);
        Task<int> GetCartItemCountAsync();
    }
}

