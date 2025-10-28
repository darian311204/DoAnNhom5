using DoAn_Backend.Models;

namespace DoAn_Backend.Services
{
    public interface IReviewService
    {
        Task<Review> AddReviewAsync(int productId, int userId, int rating, string comment);
        Task<IEnumerable<Review>> GetProductReviewsAsync(int productId);
        Task<Review?> GetReviewByIdAsync(int reviewId);
    }
}
