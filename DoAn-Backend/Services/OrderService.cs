using DoAn_Backend.Data;
using DoAn_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn_Backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(int userId, string shippingAddress, string phone, string recipientName)
        {
            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserID == userId)
                .ToListAsync();

            if (!cartItems.Any())
                throw new Exception("Cart is empty");

            var order = new Order
            {
                UserID = userId,
                OrderDate = DateTime.Now,
                ShippingAddress = shippingAddress,
                Phone = phone,
                RecipientName = recipientName,
                Status = "Pending",
                TotalAmount = 0
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            decimal totalAmount = 0;
            foreach (var cartItem in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    OrderID = order.OrderID,
                    ProductID = cartItem.ProductID,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product!.Price
                };

                totalAmount += orderDetail.UnitPrice * orderDetail.Quantity;
                _context.OrderDetails.Add(orderDetail);

                // Update stock
                var product = await _context.Products.FindAsync(cartItem.ProductID);
                if (product != null)
                {
                    product.Stock -= cartItem.Quantity;
                    _context.Products.Update(product);
                }
            }

            order.TotalAmount = totalAmount;
            _context.Orders.Update(order);

            // Clear cart
            _context.Carts.RemoveRange(cartItems);

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p!.Category)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);
        }
    }
}
