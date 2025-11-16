using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    public interface IAuthApiService
    {
        Task<AuthResponse?> LoginAsync(LoginDto dto);
        Task<AuthResponse?> RegisterAsync(RegisterDto dto);
        void Logout();
        bool IsAuthenticated();
        bool IsAdmin();
        User? GetCurrentUser();
        bool IsTokenExpired();
    }
}

