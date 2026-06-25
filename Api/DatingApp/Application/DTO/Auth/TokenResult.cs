namespace DatingApp.Application.DTO.Auth
{
    public class TokenResult
    {
        public string Token { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }
    }
}
