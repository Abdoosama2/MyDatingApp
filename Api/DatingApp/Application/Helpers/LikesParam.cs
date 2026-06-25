namespace DatingApp.Application.Helpers
{
    public class LikesParam :PagingParams
    {

        public string MemberId { get; set; } = "";

        public string Predicate { get; set; } = "Liked";
    }
}
