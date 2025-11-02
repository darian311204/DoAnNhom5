using DoAn_Frontend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DoAn_Frontend.Services
{
    public abstract class BaseApiService
    {
        protected readonly HttpClient _httpClient;
        protected readonly IConfiguration _configuration;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClient.BaseAddress = new Uri(_configuration["BackendUrl"]!);
        }

        protected string? GetToken() => _httpContextAccessor.HttpContext?.Session.GetString("Token");

        protected HttpRequestMessage CreateRequest(HttpMethod method, string url, HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, url);

            // Add authorization header if token exists
            var token = GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            if (content != null)
            {
                request.Content = content;
            }

            return request;
        }

        public bool IsTokenExpired()
        {
            try
            {
                var token = GetToken();
                if (string.IsNullOrEmpty(token)) return true;

                // Parse JWT to check expiration
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                return jwt.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }

        public bool IsAuthenticated()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return false;

            // Check if token is expired
            if (IsTokenExpired())
            {
                // Clear expired token
                Logout();
                return false;
            }

            return true;
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext?.Session.Remove("Token");
            _httpContextAccessor.HttpContext?.Session.Remove("User");
        }

        public bool IsAdmin() => GetCurrentUser()?.Role == "Admin";

        public User? GetCurrentUser()
        {
            var userJson = _httpContextAccessor.HttpContext?.Session.GetString("User");
            return string.IsNullOrEmpty(userJson) ? null : JsonSerializer.Deserialize<User>(userJson);
        }
    }
}

