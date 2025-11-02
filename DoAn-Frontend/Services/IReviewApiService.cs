using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    public interface IReviewApiService
    {
        Task<List<Review>?> GetProductReviewsAsync(int productId);
        Task<bool> AddReviewAsync(int productId, int rating, string comment);
    }
}

