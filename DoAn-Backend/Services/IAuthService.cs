using DoAn_Backend.DTOs;
using DoAn_Backend.Models;

namespace DoAn_Backend.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int userId);
        string GenerateJwtToken(User user);
    }
}
