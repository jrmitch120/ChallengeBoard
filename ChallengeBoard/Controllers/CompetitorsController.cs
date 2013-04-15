using System.Web.Mvc;
using ChallengeBoard.Infrastucture;
using ChallengeBoard.Models;

namespace ChallengeBoard.Controllers
{
    public class CompetitorsController : Controller
    {
        private readonly IRepository _repository;

        public CompetitorsController(IRepository repository)
        {
            _repository = repository;
        }

        //
        // Post: /Boards/Competitors/5
        [HttpPost]
        [Authorize]
        [AjaxOnly]
        public ActionResult Edit(int boardId, Competitor updatedCompetitor)
        {
            var competitor = _repository.GetCompetitorByName(boardId, updatedCompetitor.Name);
            var response = new JsonResponse<bool>();

            if (competitor == null)
            {
                response.Message = "Competitor not found";
                response.Error = true;
            }
            else
            {
                // Just update status for now
                competitor.Status = updatedCompetitor.Status;
                _repository.CommitChanges();
                response.Result = true;
            }

            return (Json(response));
        }
    }
}
