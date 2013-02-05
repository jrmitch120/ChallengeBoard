using System.Linq;
using System.Web.Mvc;
using ChallengeBoard.Infrastucture;
using ChallengeBoard.Models;
using ChallengeBoard.ViewModels;
using ChallengeBoard.Services;

namespace ChallengeBoard.Controllers
{
    public class MatchesController : Controller
    {
        private readonly IRepository _repository;
        private readonly IMatchService _service;

        public MatchesController(IRepository repository, IMatchService matchService)
        {
            _repository = repository;
            _service = matchService;
        }

        //
        // GET: /Matches/5

        public ActionResult List(int boardId = 0)
        {
            var board = _repository.GetBoardByIdWithCompetitors(boardId);

            if (board == null)
                return View("BoardNotFound");

            // Rejection message persisted across redirection.
            ViewBag.StatusMessage = TempData["StatusMessage"];

            return View(new RecentMatchesViewModel
            {
                Board = board,
                UnVerified = _repository.Matches.Where(m => !m.Verified && !m.Rejected && m.Board.BoardId == boardId)
                                        .OrderBy(m => m.VerificationDeadline),
                Verified =
                    _repository.Matches.Where(m => (m.Verified || m.Rejected) && m.Board.BoardId == boardId)
                               .OrderByDescending(m => m.Resolved)
                               .Take(10)
            });
        }

        //
        // GET: /Matches/Create/5

        [Authorize]
        public ActionResult Create(int boardId = 0)
        {
            var board = _repository.GetBoardByIdWithCompetitors(boardId);

            if (board == null)
                return View("BoardNotFound");

            return View(new MatchViewModel { Board = board });
        }

        // POST: /Matches/Create/5
        [HttpPost]
        [Authorize]
        //public ActionResult Create([ModelBinder(typeof(MatchBinder))]Match match)
        public ActionResult Create(int boardId, string loser, bool tie)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _service.CreateMatch(boardId, HttpContext.User.Identity.Name, loser, tie);
                }
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(new MatchViewModel
                {
                    Board = _repository.GetBoardByIdWithCompetitors(boardId),
                    Loser = loser,
                    Tie = tie
                });
            }

            return RedirectToAction("List", new { boardId });
        }

        //
        // POST: /Matches/Validate
        [HttpPost]
        [Authorize]
        [AjaxOnly]
        public ActionResult Validate(int boardId, string loser, bool tie)
        //public ActionResult Validate([ModelBinder(typeof(MatchBinder))]Match match)
        {
            var response = new JsonResponse<Match>();

            try
            {
                response.Result = _service.GenerateMatch(boardId, HttpContext.User.Identity.Name, loser, tie);
            }
            catch (ServiceException ex)
            {
                response.Message = ex.Message;
                response.Error = true;
            }

            return (Json(response));
        }

        
        // POST: /Matches/Reject/1
        [HttpPost]
        [Authorize]
        //[AjaxOnly]
        public ActionResult Reject(int boardId, int matchId)
        {
            var response = "The match has been rejected.  It will no longer count against you.";

            try
            {
                _service.RejectMatch(boardId, matchId, HttpContext.User.Identity.Name);
            }
            catch (ServiceException ex)
            {
                response = ex.Message;
            }

            // We're doing a redirect, so stuff the rejection response into TempData.
            TempData["StatusMessage"] = response;

            return RedirectToAction("List", new { boardId });
        }
    }
}
