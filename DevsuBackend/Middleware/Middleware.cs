using DB;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DevsuBackend.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, DevsuContext dbContext)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader))
            {
                // Sin header: continuar, endpoints con [Authorize] fallarán automáticamente
                await _next(context);
                return;
            }

            var parts = authHeader.Split(' ');
            if (parts.Length != 2 || !parts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(parts[1]))
            {
                throw new UnauthorizedAccessException("Formato de Authorization inválido. Use 'Bearer {token}'.");
            }

            var token = parts[1];
            await AttachUserToContext(context, dbContext, token);
            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, DevsuContext dbContext, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                if (validatedToken is not JwtSecurityToken jwt ||
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Algoritmo de token inválido");
                }

                var clienteId = int.Parse(jwt.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
                var cliente = await dbContext.Clientes.FindAsync(clienteId) ?? throw new SecurityTokenException("Usuario no encontrado");
                context.Items["Cliente"] = cliente;
            }
            catch (SecurityTokenExpiredException)
            {
                throw new UnauthorizedAccessException("Token expirado");
            }
            catch (SecurityTokenException ex)
            {
                throw new UnauthorizedAccessException($"Token inválido: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException($"Error procesando token: {ex.Message}");
            }
        }
    }
}
