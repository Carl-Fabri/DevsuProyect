using DevsuBackend.DTOs.Request;
using DevsuBackend.DTOs.Response;

namespace DevsuBackend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken);
        Task<bool> RevokeTokenAsync(string token);
    }
}
