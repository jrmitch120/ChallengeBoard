using ChallengeBoard.Models;

namespace ChallengeBoard.Services
{
    public interface IBoardService
    {
        void AdjustMatchDeadlines(Board board);
        CompetitorDiscussion GetDiscussionMeta(int boardId, string userName);
    }
}