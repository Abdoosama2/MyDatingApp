namespace DatingApp.Domain.Entites
{
    public class Photo
    {
        public int Id { get; set; }

        public  string?   Url { get; set; }

        public string? PublicId { get; set; }


        public string MemberId { get; set; } = null!;
        public Member Member { get; set; } = null!;
    }
}
