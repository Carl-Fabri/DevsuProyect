namespace DevsuBackend.DTOs.Response
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
        public ClienteDto Cliente { get; set; }
    }
}
