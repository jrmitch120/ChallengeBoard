using ChallengeBoard.Models;
using PagedList;

namespace ChallengeBoard.ViewModels
{
    public class DiscussionViewModel
    {
        public Board Board { get; set; }
        public CompetitorViewModel Viewer { get; set; }

        public IPagedList<PostViewModel> Posts { get; set; }

        public DiscussionViewModel(Board board, IPagedList<PostViewModel> posts, Competitor viewer)
        {
            Board = board;
            Viewer = new CompetitorViewModel(viewer);
            Posts = posts;
        }
    }
}