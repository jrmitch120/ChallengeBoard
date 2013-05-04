using System.Linq;
using System.Web.Mvc;
using ChallengeBoard.Infrastucture;
using ChallengeBoard.Models;
using ChallengeBoard.ViewModels;

namespace ChallengeBoard.Controllers
{
    public class CompetitorsController : Controller
    {
        private readonly IRepository _repository;

        public CompetitorsController(IRepository repository)
        {
            _repository = repository;
        }

        public new ActionResult Profile(int boardId, int competitorId)
        {
            var board = _repository.GetBoardByIdWithCompetitors(boardId);
            var competitor = board.Competitors.FindCompetitorById(competitorId);
            //_repository.GetCompetitorById(boardId, competitorId);

            if (competitor == null)
                return View("CompetitorNotFound", board);

            var recentMatches = _repository.Matches.Where(m => (m.Verified || m.Rejected) &&
                                                               (m.Winner.CompetitorId == competitorId ||
                                                                m.Loser.CompetitorId == competitorId) &&
                                                               m.Board.BoardId == boardId
                )
                                           .OrderByDescending(m => m.Resolved)
                                           .Take(300);

            return View(new ProfileViewModel {Board = board, Competitor = competitor, Matches = recentMatches});
        }

        //
        // Post: /Boards/Competitors/5
        [HttpPost]
        [Authorize]
        [AjaxOnly]
        public ActionResult Edit(int boardId, Competitor updatedCompetitor)
        {
            var board = _repository.GetBoardById(boardId);
            var competitor = _repository.GetCompetitorByName(boardId, updatedCompetitor.Name);                
            var response = new JsonResponse<bool>();

            if (competitor == null)
            {
                response.Message = "Competitor not found";
                response.Error = true;
            }
            else if (!competitor.CanEdit(board, User.Identity.Name))
            {
                response.Message = "Invalid authority";
                response.Error = true;
            }
            else
            {
                // Just update status for now (owner only can change status)
                if(board.IsOwner(User.Identity.Name))
                    competitor.Status = updatedCompetitor.Status;

                _repository.CommitChanges();
                response.Result = true;
            }

            return (Json(response));
        }
    }
}
