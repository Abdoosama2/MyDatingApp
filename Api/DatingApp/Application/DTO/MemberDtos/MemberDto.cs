namespace DatingApp.Application.DTO.MemberDtos
{
    public class MemberDto
    {

        public string Id { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Descriptions { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public int Age { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        //public List<PhotoDto> Photos { get; set; } = [];
    }
}
