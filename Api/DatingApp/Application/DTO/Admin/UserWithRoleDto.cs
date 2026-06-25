namespace DatingApp.Application.DTO.Admin
{
    public class UserWithRoleDto
    {
        public string Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = [];
    }
}
