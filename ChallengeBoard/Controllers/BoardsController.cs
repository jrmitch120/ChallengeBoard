using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using PagedList;
using ChallengeBoard.Infrastucture;
using ChallengeBoard.Models;
using ChallengeBoard.Services;
using ChallengeBoard.ViewModels;

namespace ChallengeBoard.Controllers
{
    //[MetaRequest("id")] Somethins is screwed up with the context here.  Getting stale data.
    public class BoardsController : Controller
    {
        private readonly IRepository _repository;
        private readonly IBoardService _boardService;

        public BoardsController(IRepository repository, IBoardService boardService)
        {
            _repository = repository;
            _boardService = boardService;
        }

        //
        // GET: /Boards/

        public ActionResult Index(string user = "", int page = 1)
        {
            var boards = !user.IsEmpty()
                             ? _repository.GetBoardsForCompetitor(User.Identity.Name)
                             : _repository.Boards.Where(x => x.End > DateTime.Now);

            // Persist any status messages across redirection.
            ViewBag.StatusMessage = TempData["StatusMessage"];

            return View(new BoardListViewModel(boards.OrderByDescending(x => x.End).Skip(Math.Abs(page - 1)).Take(100).ToList()));
        }

        //
        // GET: /Boards/Search?search=Test+Search[&user=Tom]

        [AjaxOnly]
        public ActionResult Search(string search, string user = "")
        {
            var boards = !user.IsEmpty()
                             ? _repository.GetBoardsForCompetitor(User.Identity.Name)
                             : _repository.Boards.Where(x => x.End > DateTime.Now);

            // If no search query is provided, select all
            if(String.IsNullOrWhiteSpace(search) == false)
                boards = boards.Where(x => x.Name.ToLower().Contains(search.ToLower().Trim()));
            boards = boards.OrderByDescending(x => x.End).Take(100);

            return (Json(new BoardListViewModel(boards), JsonRequestBehavior.AllowGet));
        }

        //
        // GET: /Boards/Details/5

        public ActionResult Details(int id = 0)
        {
            ViewBag.StatusMessage = TempData["StatusMessage"];

            var board = _repository.GetBoardByIdWithCompetitors(id);

            if (board == null)
                return View("BoardNotFound");

            return View(board);
        }

        //
        // GET: /Boards/Create

        [Authorize]
        public ActionResult Create()
        {
            return View(new Board());
        }

        //
        // POST: /Boards/Create

        [HttpPost]
        [Authorize]
        public ActionResult Create(Board board)
        {
            if (ModelState.IsValid)
            {
                var owner = new Competitor
                {
                    Name = User.Identity.Name,
                    Rating = board.StartingRating,
                    Profile = _repository.UserProfiles.FindProfile(User.Identity.Name)
                };

                _repository.Add(owner);
                _repository.CommitChanges();

                board.Created = DateTime.Now;
                board.Owner = owner;
                board.Competitors.Add(owner);

                _repository.Add(board);
                _repository.CommitChanges();

                return RedirectToAction("Index");
            }

            return View(board);
        }

        // 
        // GET: /Boards/Instructions/5
        [Authorize]
        public ActionResult Instructions(int id = 0)
        {
            var existingBoard = _repository.GetBoardByIdWithCompetitors(id);

            if (existingBoard == null)
                return View("BoardNotFound");

            return View("Instructions", existingBoard);
        }

        // 
        // GET: /Boards/Join/1
        [Authorize]
        public ActionResult Join(int id = 0)
        {
            var existingBoard = _repository.GetBoardByIdWithCompetitors(id);

            if (existingBoard == null)
                return View("BoardNotFound");

            if (existingBoard.Password.IsEmpty() ||
                existingBoard.Competitors.Active().ContainsCompetitor(User.Identity.Name))
                return RedirectToAction("Details", new { id = existingBoard.BoardId });

            return View("Join", existingBoard);
        }

        // 
        // POST: /Boards/Join/5
        [HttpPost, ActionName("Join")]
        [Authorize]
        public ActionResult JoinBoard(int id, string password = "")
        {
            // TODO, move this into a BoardService
            var existingBoard = _repository.GetBoardByIdWithCompetitors(id);

            // Password failure
            if (!existingBoard.Password.IsEmpty() &&
                !existingBoard.Password.Equals(password, StringComparison.InvariantCultureIgnoreCase))
            {
                ModelState.AddModelError("invalidPassword", "The password you entered was incorrect");
                return View("Join", existingBoard);
            }

            var userProfile = _repository.UserProfiles.FindProfile(User.Identity.Name);
            var competitor = existingBoard.Competitors.FindCompetitorByProfileId(userProfile.UserId);

            if (competitor == null) // New
            {
                existingBoard.Competitors.Add(new Competitor
                {
                    Name = User.Identity.Name,
                    Rating = existingBoard.StartingRating,
                    Profile = userProfile
                });
            }
            else if (competitor.Status == CompetitorStatus.Retired) // Retired
                competitor.Status = CompetitorStatus.Active;
            else
                return View("Banned", existingBoard); // Banned

            _repository.CommitChanges();

            return RedirectToAction("Instructions", new { id = existingBoard.BoardId });
        }

        //
        // GET: /Boards/Edit/5

        [Authorize]
        public ActionResult Edit(int id = 0)
        {
            var board = _repository.GetBoardByIdWithCompetitors(id);
            
            if (board == null)
                return View("BoardNotFound");

            if (!board.IsOwner(User.Identity.Name))
                return View("InvalidOwner", board);

            board.Competitors.ToList().ForEach(c => c.Profile.EmailAddress = string.Empty);

            return View(board);
        }

        //
        // POST: /Boards/Edit/5

        [HttpPost]
        //public ActionResult Edit(int id, FormCollection formValues)
        public ActionResult Edit(int id, Board userBoard)
        {
            var board = _repository.GetBoardById(id);

            if (board == null)
                return View("BoardNotFound");

            if (ModelState.IsValid)
            {
                if (!board.IsOwner(User.Identity.Name))
                    return View("InvalidOwner", board);

                if (userBoard.AutoVerification != board.AutoVerification)
                {
                    userBoard.Matches = _repository.GetUnresolvedMatchesByBoardId(userBoard.BoardId).ToList();
                    _boardService.AdjustMatchDeadlines(userBoard);
                }

                TryUpdateModel(board);

                _repository.CommitChanges(true);

                TempData["StatusMessage"] = "Your changes have been saved.";

                return RedirectToAction("Details", new { id = userBoard.BoardId });
            }

            return View(board);
        }

        //
        // GET: /Boards/Delete/5

        [Authorize]
        public ActionResult Delete(int id = 0)
        {
            var board = _repository.GetBoardById(id);

            if (_repository.Matches.Any(x => x.Board.BoardId == id))
                return View("CanNotDelete", board);

            if (board == null)
                return View("BoardNotFound");

            if (!board.IsOwner(User.Identity.Name))
                return View("InvalidOwner", board);

            return View(board);
        }

        //
        // POST: /Boards/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            var board = _repository.GetBoardById(id);

            if (_repository.Matches.Any(x => x.Board.BoardId == id))
                return View("CanNotDelete", board);

            if (board == null)
                return View("BoardNotFound");

            if (!board.IsOwner(User.Identity.Name))
                return View("InvalidOwner", board);

            board.Owner = null;
            _repository.CommitChanges();

            _repository.Delete(board);
            _repository.CommitChanges();

            return RedirectToAction("Index");
        }


        [Authorize]
        public ActionResult Retire(int id)
        {
            var existingBoard = _repository.GetBoardByIdWithCompetitors(id);

            if (existingBoard == null)
                return View("BoardNotFound");

            if (existingBoard.Competitors.Active().ContainsCompetitor(User.Identity.Name))
                return View("Retire", existingBoard);

            return RedirectToAction("Index");
        }

        // 
        // POST: /Boards/Retire/1
        [HttpPost, ActionName("Retire")]
        [Authorize]
        public ActionResult RetireConfirmed(int id)
        {
            var existingBoard = _repository.GetBoardById(id);

            var competitor = _repository.GetCompetitorByUserName(id, User.Identity.Name);

            if (competitor == null)
                return RedirectToAction("Index");

            if (existingBoard.IsOwner(competitor.Name))
                return View("Unleavable", existingBoard);

            if (competitor.Status == CompetitorStatus.Active)
            {
                competitor.Status = CompetitorStatus.Retired;
                _repository.CommitChanges();
            }

            // Rejection message persisted across redirection.
            TempData["StatusMessage"] = String.Format("You have retired from {0}", existingBoard.Name);

            return RedirectToAction("Index");
        }

        //
        // Get: /Boards/Standings/5

        public ActionResult Standings(int id = 0, int page = 1, bool official = true)
        {
            if (page < 1) page = 1;

            var existingBoard = _repository.GetBoardByIdWithCompetitors(id);

            if (existingBoard == null)
                return View("BoardNotFound");

            if (!official)
            {
                // All unresolved matches for this challenge board.
                var unresolvedMatches = _repository.GetUnresolvedMatchesByBoardId(id);

                var deltas = new Dictionary<int, Dictionary<string, int>>();

                // Traverse the pending queue in order and build competitor deltas dictionary
                foreach (var match in unresolvedMatches.OrderBy(x => x.VerificationDeadline))
                {
                    if (!deltas.ContainsKey(match.Winner.CompetitorId))
                        deltas.Add(match.Winner.CompetitorId, new Dictionary<string, int>{{"rating", 0},{"wins", 0},{"loses", 0},{"ties", 0},{"streak", 0}});

                    if (!deltas.ContainsKey(match.Loser.CompetitorId))
                        deltas.Add(match.Loser.CompetitorId, new Dictionary<string, int>{{"rating", 0},{"wins", 0},{"loses", 0},{"ties", 0},{"streak", 0}});

                    deltas[match.Winner.CompetitorId]["rating"] += match.WinnerRatingDelta;
                    deltas[match.Loser.CompetitorId]["rating"] += match.LoserRatingDelta;

                    if (match.Tied)
                    {
                        deltas[match.Winner.CompetitorId]["ties"] += 1;
                        deltas[match.Loser.CompetitorId]["ties"] += 1;
                        deltas[match.Winner.CompetitorId]["streak"] = 0;
                    }
                    else
                    {
                        deltas[match.Loser.CompetitorId]["loses"] += 1;
                        deltas[match.Loser.CompetitorId]["streak"] = 0;

                        deltas[match.Winner.CompetitorId]["wins"] += 1;
                        deltas[match.Winner.CompetitorId]["streak"] += 1;
                    }
                }

                // Traverse existingBoard.Competitors and apply deltas gathered in the dictionary
                foreach (var competitor in existingBoard.Competitors)
                {
                    if (deltas.ContainsKey(competitor.CompetitorId))
                    {
                        competitor.Rating += deltas[competitor.CompetitorId]["rating"];
                        competitor.Wins += deltas[competitor.CompetitorId]["wins"];
                        competitor.Loses += deltas[competitor.CompetitorId]["loses"];
                        competitor.Ties += deltas[competitor.CompetitorId]["ties"];
                        competitor.Streak += deltas[competitor.CompetitorId]["streak"];
                    }
                }
            }

            // Unranked players get 0 rating.
            existingBoard.Competitors.Where(c => c.MatchesPlayed == 0).ToList().ForEach(c => c.Rating = 0);

            return View("Standings", new StandingsViewModel
            {
                Board = existingBoard,
                Standings = existingBoard.Competitors.Active().OrderByDescending(c => c.Rating).ToPagedList(page, PageLimits.Standings)
            });
        }

        //
        // Get: /Boards/ProvisionalStandings/5

        public ActionResult ProvisionalStandings(int id = 0, int page = 1)
        {
            return Standings(id, page, false);
        }

    }
}
