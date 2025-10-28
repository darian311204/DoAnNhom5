using DoAn_Backend.Models;

namespace DoAn_Backend.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(int userId, string shippingAddress, string phone, string recipientName);
        Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int orderId);
    }
}
