using ChallengeBoard.Models;
using PagedList;

namespace ChallengeBoard.ViewModels
{
    public class DiscussionViewModel
    {
        public Board Board { get; set; }
        public CompetitorViewModel Viewer { get; set; }

        public int FocusPostId { get; set; }
        public IPagedList<PostViewModel> Posts { get; set; }

        public DiscussionViewModel(Board board, IPagedList<PostViewModel> posts, int focusPostId, Competitor viewer)
        {
            Board = board;
            FocusPostId = focusPostId;
            Viewer = viewer != null && viewer.Status == CompetitorStatus.Active
                         ? new CompetitorViewModel(viewer)
                         : new CompetitorViewModel();

            Posts = posts;
        }
    }
}