using DB.Models;
using System.Security.Claims;

namespace DevsuBackend.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(Cliente cliente);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
