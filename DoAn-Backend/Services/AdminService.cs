using DoAn_Backend.Data;
using DoAn_Backend.DTOs;
using DoAn_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn_Backend.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Products
        public async Task<Product> CreateProductAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                ProductName = dto.ProductName,
                Price = dto.Price,
                Description = dto.Description,
                ImageURL = dto.ImageURL,
                CategoryID = dto.CategoryID,
                Stock = dto.Stock,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new Exception("Product not found");

            product.ProductName = dto.ProductName;
            product.Price = dto.Price;
            product.Description = dto.Description;
            product.ImageURL = dto.ImageURL;
            product.CategoryID = dto.CategoryID;
            product.Stock = dto.Stock;
            product.IsActive = dto.IsActive;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            // Soft delete
            product.IsActive = false;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }

        // Categories
        public async Task<Category> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                CategoryName = dto.CategoryName,
                Description = dto.Description,
                IsActive = dto.IsActive
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                throw new Exception("Category not found");

            category.CategoryName = dto.CategoryName;
            category.Description = dto.Description;
            category.IsActive = dto.IsActive;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null) return false;

            // Soft delete
            category.IsActive = false;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        // Users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "Customer")
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ToggleUserStatusAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsActive = !user.IsActive;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Orders
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, string? cancelReason = null)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            if (status == "Cancelled" && !string.IsNullOrWhiteSpace(cancelReason))
            {
                order.CancelReason = cancelReason;
            }
            else if (status != "Cancelled")
            {
                order.CancelReason = null; // Clear cancel reason if status is not Cancelled
            }
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }

        // Reviews
        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ToggleReviewStatusAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            review.IsActive = !review.IsActive;
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return true;
        }

        // Statistics
        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Orders.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate <= endDate.Value);

            return await query.SumAsync(o => o.TotalAmount);
        }

        public async Task<IEnumerable<object>> GetOrdersByDateAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    OrderCount = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount)
                })
                .ToListAsync();
        }
    }
}
