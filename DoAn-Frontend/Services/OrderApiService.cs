using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    public class OrderApiService : BaseApiService, IOrderApiService
    {
        public OrderApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, configuration, httpContextAccessor)
        {
        }

        public async Task<Order?> CreateOrderAsync(string shippingAddress, string phone, string recipientName)
        {
            var content = JsonContent.Create(new { shippingAddress, phone, recipientName });
            var request = CreateRequest(HttpMethod.Post, "Orders/checkout", content);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Order>() : null;
        }

        public async Task<List<Order>?> GetUserOrdersAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "Orders/history");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<Order>>() : new List<Order>();
        }
    }
}

