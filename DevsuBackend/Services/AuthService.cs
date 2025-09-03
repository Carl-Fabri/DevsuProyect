using DB;
using DevsuBackend.DTOs;
using DevsuBackend.DTOs.Request;
using DevsuBackend.DTOs.Response;
using DevsuBackend.Services.Interfaces;
using DevsuBackend.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;

namespace DevsuBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly DevsuContext _context;
        private readonly IJwtService _jwtService;
        private readonly IEncryptionHelper _encryptionHelper;
        private readonly IConfiguration _configuration;

        public AuthService(DevsuContext context, IJwtService jwtService, IEncryptionHelper encryptionHelper, IConfiguration configuration)
        {
            _context = context;
            _jwtService = jwtService;
            _encryptionHelper = encryptionHelper;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Identificacion == loginRequest.Identificacion && c.Estado);

            if (cliente == null || !_encryptionHelper.VerifyPassword(loginRequest.Contrasena, cliente.Contrasena))
                throw new UnauthorizedAccessException("Credenciales inválidas");

            var token = _jwtService.GenerateJwtToken(cliente);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Guardar refresh token en base de datos
            cliente.RefreshToken = refreshToken;
            cliente.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),
                RefreshToken = refreshToken,
                Cliente = new ClienteDto
                {
                    ClienteId = cliente.ClienteId,
                    Nombre = cliente.Nombre,
                    Identificacion = cliente.Identificacion
                }
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(token);
            var clienteId = int.Parse(principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null || cliente.RefreshToken != refreshToken || cliente.RefreshTokenExpiry <= DateTime.UtcNow)
                throw new SecurityTokenException("Invalid refresh token");

            var newJwtToken = _jwtService.GenerateJwtToken(cliente);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            cliente.RefreshToken = newRefreshToken;
            cliente.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = newJwtToken,
                Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),
                RefreshToken = newRefreshToken
            };
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(token);
            var clienteId = int.Parse(principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null) return false;

            cliente.RefreshToken = null;
            cliente.RefreshTokenExpiry = null;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
