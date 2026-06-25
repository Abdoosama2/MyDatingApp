using System.ComponentModel.DataAnnotations;

namespace DatingApp.Application.DTO
{
    public class RegisterDto
    {

        [Required]
        [EmailAddress]

        public string Email { get; set; } = "";
        [Required]
        public  string DisplayName { get; set; }="";

        [Required]
        public string Password { get; set; } = "";
        [Required]
        public string Gender { get; set; } = string.Empty;
        [Required]
        public string City { get; set; } = string.Empty;
        [Required]
        public string Country { get; set; } = string.Empty;
        [Required]
        public DateOnly DateOfBirth { get; set; }
    }
    
    
}
