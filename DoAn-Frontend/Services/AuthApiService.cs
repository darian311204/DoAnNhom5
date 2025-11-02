using DoAn_Frontend.Models;
using System.Text.Json;

namespace DoAn_Frontend.Services
{
    public class AuthApiService : BaseApiService, IAuthApiService
    {
        public AuthApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, configuration, httpContextAccessor)
        {
        }

        public async Task<AuthResponse?> LoginAsync(LoginDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Auth/login", dto);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    if (result != null)
                    {
                        // Store JWT token in session
                        _httpContextAccessor.HttpContext?.Session.SetString("Token", result.Token);
                        _httpContextAccessor.HttpContext?.Session.SetString("User", JsonSerializer.Serialize(result.User));
                    }
                    return result;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Login failed: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
            }
            return null;
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Auth/register", dto);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    // Don't store session after registration - let user log in manually
                    return result;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Registration failed: {response.StatusCode} - {errorContent}");
                    throw new Exception(errorContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
                throw; // Re-throw to be caught by controller
            }
        }
    }
}

