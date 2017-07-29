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

            var userProfile = _repository.UserProfiles.FindProfile(User.Identity.Name);

            return View(new RecentMatchesViewModel
            {
                Board = board,
                Viewer = userProfile != null ? board.Competitors.FindCompetitorByProfileId(userProfile.UserId) : null,

                UnVerified = _repository.GetUnresolvedMatchesByBoardId(boardId)
                                        .OrderByDescending(m => m.VerificationDeadline)
                                        .Take(300), // Limit for now.  > 300 pending should be rare.  TODO load more on demand.

                Verified = _repository.GetResolvedMatchesByBoardId(boardId)
                                      .OrderByDescending(m => m.Created)
                                      .Take(50)
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
        public ActionResult Create(int boardId, MatchViewModel match)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _service.CreateMatch(boardId, HttpContext.User.Identity.Name, match.Loser, match.WinnerComment,
                                         match.Tie);
                }
            }
            catch (ServiceException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(new MatchViewModel
                {
                    Board = _repository.GetBoardByIdWithCompetitors(boardId),
                    Loser = match.Loser,
                    WinnerComment = match.WinnerComment,
                    Tie = match.Tie
                });
            }

            TempData["StatusMessage"] = "Your match has been successfully reported.";

            return RedirectToAction("List", new { boardId });
        }

        //
        // POST: /Matches/Validate
        [HttpPost]
        [Authorize]
        [AjaxOnly]
        public ActionResult Validate(int boardId, MatchViewModel match)
        //public ActionResult Validate([ModelBinder(typeof(MatchBinder))]Match match)
        {
            var response = new JsonResponse<Match>();

            try
            {
                response.Result = _service.GenerateMatch(boardId, HttpContext.User.Identity.Name, match.Loser, match.Tie);
                response.Result.WinnerComment = match.WinnerComment;
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
            var response = "The match has been invalidated.  It will no longer be counted in the standings.";

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

        // POST: /Matches/Finalize/1
        [HttpPost]
        [Authorize]
        public ActionResult Finalize(int boardId, int matchId, string finalizationRequest)
        {
            string response;

            try
            {
                if (finalizationRequest.Equals("verify", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    response = "Your match has been marked as verified.  It will enter the approval queue and " +
                               "be processed as soon as there are no pending matches for either competitor in front of it.  " +
                               "The approval queue is processed approximately every 30 seconds.";
                    _service.ConfirmMatch(boardId, matchId, HttpContext.User.Identity.Name);
                }
                else
                {
                    response = "The match has been invalidated.  It will no longer be counted in the standings.";
                    _service.RejectMatch(boardId, matchId, HttpContext.User.Identity.Name);
                }
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
