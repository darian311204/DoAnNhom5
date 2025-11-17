using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    public class ReviewApiService : BaseApiService, IReviewApiService
    {
        public ReviewApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, configuration, httpContextAccessor)
        {
        }

        // Public endpoints - không cần authentication
        public async Task<List<Review>?> GetProductReviewsAsync(int productId)
        {
            var response = await _httpClient.GetAsync($"Reviews/product/{productId}");
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<Review>>() : new List<Review>();
        }

        public async Task<ReviewStatistics?> GetReviewStatisticsAsync(int productId)
        {
            var response = await _httpClient.GetAsync($"Reviews/statistics/{productId}");
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<ReviewStatistics>() : null;
        }

        // User endpoint - cần authentication
        public async Task<bool> AddReviewAsync(int productId, int rating, string comment)
        {
            var content = JsonContent.Create(new { productId, rating, comment });
            var request = CreateRequest(HttpMethod.Post, "Reviews", content);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Admin endpoints - cần admin authentication
        public async Task<List<Review>?> GetAllReviewsAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "admin/AdminReview/all");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<Review>>() : null;
        }

        public async Task<bool> ToggleReviewActiveAsync(int reviewId)
        {
            var request = CreateRequest(HttpMethod.Put, $"admin/AdminReview/{reviewId}/toggle-active");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            var request = CreateRequest(HttpMethod.Delete, $"admin/AdminReview/{reviewId}");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
