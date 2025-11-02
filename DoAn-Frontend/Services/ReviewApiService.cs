using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    public class ReviewApiService : BaseApiService, IReviewApiService
    {
        public ReviewApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, configuration, httpContextAccessor)
        {
        }

        public async Task<List<Review>?> GetProductReviewsAsync(int productId)
        {
            var response = await _httpClient.GetAsync($"Reviews/product/{productId}");
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<Review>>() : new List<Review>();
        }

        public async Task<bool> AddReviewAsync(int productId, int rating, string comment)
        {
            var content = JsonContent.Create(new { productId, rating, comment });
            var request = CreateRequest(HttpMethod.Post, "Reviews", content);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}

