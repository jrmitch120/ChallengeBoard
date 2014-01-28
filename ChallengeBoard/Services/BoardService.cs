using System.Linq;
using ChallengeBoard.Models;

namespace ChallengeBoard.Services
{
    public class BoardService : IBoardService
    {
        private readonly IRepository _repository;

        public BoardService(IRepository repository)
        {
            _repository = repository;
        }

        public void AdjustMatchDeadlines(Board board)
        {
            foreach (var match in board.Matches.Where(match => !match.IsResolved))
            {
                match.VerificationDeadline = match.Created.AddHours(board.AutoVerification);
            }
        }

        public CompetitorDiscussion GetDiscussionMeta(int boardId, string userName)
        {
            var competitor = _repository.GetCompetitorByUserName(boardId, userName);
            if (competitor == null) return (new CompetitorDiscussion());

            // TODO: Is there a way to get this in one request?
            var lastPostId = _repository.Posts.Where(p => p.Board.BoardId == boardId).Max(p => (int?)p.PostId) ?? 0;
            var newPostCount =
                _repository.Posts.Count(p => p.Board.BoardId == boardId && p.PostId > competitor.LastViewedPostId);
            
            return
                (new CompetitorDiscussion
                {
                    LatestPostId = lastPostId,
                    LastViewedPostId = competitor.LastViewedPostId,
                    NumberOfNewPosts = newPostCount
                });
        }
    }
}