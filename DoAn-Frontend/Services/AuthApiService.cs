using DoAn_Frontend.Models;
using System.Text.Json;
using System.Text;

namespace DoAn_Frontend.Services
{
    public class AuthApiService : BaseApiService, IAuthApiService
    {
        public AuthApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, configuration, httpContextAccessor)
        {
        }

        //public async Task<AuthResponse?> LoginAsync(LoginDto dto)
        //{
        //    try
        //    {
        //        var response = await _httpClient.PostAsJsonAsync("Auth/login", dto);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        //            if (result != null)
        //            {
        //                // Store JWT token in session
        //                _httpContextAccessor.HttpContext?.Session.SetString("Token", result.Token);
        //                _httpContextAccessor.HttpContext?.Session.SetString("User", JsonSerializer.Serialize(result.User));
        //            }
        //            return result;
        //        }
        //        else
        //        {
        //            var errorContent = await response.Content.ReadAsStringAsync();
        //            Console.WriteLine($"Login failed: {response.StatusCode} - {errorContent}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Login error: {ex.Message}");
        //    }
        //    return null;
        //}
        //public async Task<AuthResponse?> LoginAsync(LoginDto dto)
        //{
        //    try
        //    {
        //        var json = JsonSerializer.Serialize(dto);
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //        // ✅ SỬ DỤNG CreateRequest() thay vì PostAsJsonAsync
        //        var request = CreateRequest(HttpMethod.Post, "Auth/login", content);
        //        var response = await _httpClient.SendAsync(request);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var resultJson = await response.Content.ReadAsStringAsync();
        //            var result = JsonSerializer.Deserialize<AuthResponse>(resultJson,
        //                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //            if (result != null)
        //            {
        //                // Store JWT token in session
        //                _httpContextAccessor.HttpContext?.Session.SetString("Token", result.Token);
        //                _httpContextAccessor.HttpContext?.Session.SetString("User", JsonSerializer.Serialize(result.User));

        //                // DEBUG: Verify token was saved
        //                var savedToken = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        //                Console.WriteLine($"[LoginAsync] Token saved: {!string.IsNullOrEmpty(savedToken)}, Length: {savedToken?.Length ?? 0}");
        //            }
        //            return result;
        //        }
        //        else
        //        {
        //            var errorContent = await response.Content.ReadAsStringAsync();
        //            Console.WriteLine($"Login failed: {response.StatusCode} - {errorContent}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Login error: {ex.Message}");
        //    }
        //    return null;
        //}
        public async Task<AuthResponse?> LoginAsync(LoginDto dto)
        {
            try
            {
                Console.WriteLine("[AuthApiService] LoginAsync START");

                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = CreateRequest(HttpMethod.Post, "Auth/login", content);
                var response = await _httpClient.SendAsync(request);

                Console.WriteLine($"[AuthApiService] Login response status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var resultJson = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<AuthResponse>(resultJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (result != null)
                    {
                        Console.WriteLine($"[AuthApiService] Result token length: {result.Token?.Length ?? 0}");
                        Console.WriteLine($"[AuthApiService] HttpContext is null: {_httpContextAccessor.HttpContext == null}");
                        Console.WriteLine($"[AuthApiService] Session is null: {_httpContextAccessor.HttpContext?.Session == null}");

                        // Store JWT token in session
                        _httpContextAccessor.HttpContext?.Session.SetString("Token", result.Token);
                        _httpContextAccessor.HttpContext?.Session.SetString("User", JsonSerializer.Serialize(result.User));

                        // CRITICAL: Commit session changes
                        await _httpContextAccessor.HttpContext!.Session.CommitAsync();

                        // DEBUG: Verify token was saved
                        var savedToken = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                        var savedUser = _httpContextAccessor.HttpContext?.Session.GetString("User");
                        Console.WriteLine($"[AuthApiService] Token saved verification: {!string.IsNullOrEmpty(savedToken)}, Length: {savedToken?.Length ?? 0}");
                        Console.WriteLine($"[AuthApiService] User saved verification: {!string.IsNullOrEmpty(savedUser)}");
                        Console.WriteLine($"[AuthApiService] Session ID after save: {_httpContextAccessor.HttpContext?.Session.Id}");
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
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            return null;
        }
        //public async Task<AuthResponse?> RegisterAsync(RegisterDto dto)
        //{
        //    try
        //    {
        //        var response = await _httpClient.PostAsJsonAsync("Auth/register", dto);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        //            // Don't store session after registration - let user log in manually
        //            return result;
        //        }
        //        else
        //        {
        //            var errorContent = await response.Content.ReadAsStringAsync();
        //            Console.WriteLine($"Registration failed: {response.StatusCode} - {errorContent}");
        //            throw new Exception(errorContent);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Registration error: {ex.Message}");
        //        throw; // Re-throw to be caught by controller
        //    }
        //}
        public async Task<AuthResponse?> RegisterAsync(RegisterDto dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = CreateRequest(HttpMethod.Post, "Auth/register", content);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var resultJson = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<AuthResponse>(resultJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
                throw;
            }
        }

        // The following methods are inherited from BaseApiService,
        // but explicitly implement the interface for clarity.
        public new void Logout() => base.Logout();
        public new bool IsAuthenticated() => base.IsAuthenticated();
        public new bool IsAdmin() => base.IsAdmin();
        public new User? GetCurrentUser() => base.GetCurrentUser();
        public new bool IsTokenExpired() => base.IsTokenExpired();
    }
}

