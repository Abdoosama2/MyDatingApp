namespace DatingApp.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IMemberRepository memberRepository { get; }

        IMessageRepository messageRepository { get; }

        ILikeRepository likeRepository { get; }

        Task<bool> Complete();

        bool HasChanges();
    }
}
