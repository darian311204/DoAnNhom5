using DoAn_Backend.Models;

namespace DoAn_Backend.Services
{
    public interface IReviewService
    {
        Task<Review> AddReviewAsync(int productId, int userId, int rating, string comment);
        Task<IEnumerable<Review>> GetProductReviewsAsync(int productId);
        Task<Review?> GetReviewByIdAsync(int reviewId);
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task<bool> ToggleReviewActiveAsync(int reviewId);
        Task<bool> DeleteReviewAsync(int reviewId);
        Task<ReviewStatistics> GetReviewStatisticsAsync(int productId);
        Task<bool> UserHasReviewedProductAsync(int productId, int userId);
    }

    public class ReviewStatistics
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
    }
}