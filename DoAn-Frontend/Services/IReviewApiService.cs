using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    public interface IReviewApiService
    {
        Task<List<Review>?> GetProductReviewsAsync(int productId);
        Task<bool> AddReviewAsync(int productId, int rating, string comment);
        Task<List<Review>?> GetAllReviewsAsync();
        Task<bool> ToggleReviewActiveAsync(int reviewId);
        Task<bool> DeleteReviewAsync(int reviewId);
        Task<ReviewStatistics?> GetReviewStatisticsAsync(int productId);
    }
}