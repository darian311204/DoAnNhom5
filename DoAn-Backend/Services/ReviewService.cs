using DoAn_Backend.Data;
using DoAn_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn_Backend.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Review> AddReviewAsync(int productId, int userId, int rating, string comment)
        {
            var review = new Review
            {
                ProductID = productId,
                UserID = userId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<IEnumerable<Review>> GetProductReviewsAsync(int productId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductID == productId && r.IsActive)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(int reviewId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.ReviewID == reviewId);
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ToggleReviewActiveAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
                return false;

            review.IsActive = !review.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
                return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ReviewStatistics> GetReviewStatisticsAsync(int productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductID == productId && r.IsActive)
                .ToListAsync();

            var stats = new ReviewStatistics
            {
                TotalReviews = reviews.Count,
                AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0
            };

            for (int i = 1; i <= 5; i++)
            {
                stats.RatingDistribution[i] = reviews.Count(r => r.Rating == i);
            }

            return stats;
        }

        public async Task<bool> UserHasReviewedProductAsync(int productId, int userId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.ProductID == productId && r.UserID == userId);
        }
    }
}