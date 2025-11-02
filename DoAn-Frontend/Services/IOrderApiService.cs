using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    public interface IOrderApiService
    {
        Task<Order?> CreateOrderAsync(string shippingAddress, string phone, string recipientName);
        Task<List<Order>?> GetUserOrdersAsync();
    }
}

