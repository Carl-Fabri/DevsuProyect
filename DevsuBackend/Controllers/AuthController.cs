using DevsuBackend.DTOs.Request;
using DevsuBackend.DTOs.Response;
using DevsuBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DevsuBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        private ActionResult Error(string code, string message, int statusCode)
        {
            return StatusCode(statusCode, new
            {
                error = code,
                message,
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (!ModelState.IsValid)
                return Error("validation_error", "Datos inválidos", StatusCodes.Status400BadRequest);

            try
            {
                var response = await _authService.LoginAsync(loginRequest);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Error("unauthorized", ex.Message, StatusCodes.Status401Unauthorized);
            }
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshTokenRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.RefreshToken))
                return Error("validation_error", "Token y refresh token son requeridos", StatusCodes.Status400BadRequest);

            try
            {
                var response = await _authService.RefreshTokenAsync(request.Token, request.RefreshToken);
                return Ok(response);
            }
            catch (SecurityTokenException ex)
            {
                return Error("invalid_token", ex.Message, StatusCodes.Status401Unauthorized);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Error("unauthorized", ex.Message, StatusCodes.Status401Unauthorized);
            }
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrWhiteSpace(token))
                return Error("validation_error", "Bearer token requerido", StatusCodes.Status400BadRequest);

            var result = await _authService.RevokeTokenAsync(token);
            return result ? Ok(new { success = true }) : Error("not_found", "Token no encontrado o ya revocado", StatusCodes.Status404NotFound);
        }
    }
}
