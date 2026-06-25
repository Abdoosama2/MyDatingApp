using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;

namespace DatingApp.Application.DTO.Auth
{
    public class AuthResponse
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }

         public string Token { get; set; }

          public List<string> Roles { get; set; }
         public DateTime? Expiration { get; set; }

          public string? ImageUrl { get; set; }

          [JsonIgnore]
          public string RefreshToken { get; set; }
         


    }
}
