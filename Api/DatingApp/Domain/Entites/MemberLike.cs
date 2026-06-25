namespace DatingApp.Domain.Entites
{
    public class MemberLike
    {

        public  string SourceMemberId { get; set; }

        public Member SourceMember { get; set; }

        public string TargetMemberId { get; set; }

        public Member TargetMember  { get; set; }
    }
}
