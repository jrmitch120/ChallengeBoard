using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;

namespace ChallengeBoard.Models
{
    public class PersistedRepository : IRepository
    {
        public readonly ChallengeBoardContext Db;

        public PersistedRepository(ChallengeBoardContext db)
        {
            Db = db;    
        }

        public IQueryable<Board> Boards
        {
            get { return (Db.Boards.Include(c => c.Owner.Profile)); }
        }

        public IQueryable<Competitor> Competitors
        {
            get { return (Db.Competitors.Include(c => c.Profile)); }
        }

        public IQueryable<Match> Matches
        {
            get { return (Db.Matches.Include(c => c.Winner).Include(c => c.Loser)); }
        }

        public IQueryable<Post> Posts
        {
            get { return (Db.Posts.Include(c => c.Owner.Profile)); }
        }

        public IQueryable<UserProfile> UserProfiles
        {
            get { return (Db.UserProfiles); }
            
        }

        public void Add(Board board)
        {
            Db.Boards.Add(board);
        }

        public void Add(Competitor competitor)
        {
            Db.Competitors.Add(competitor);
        }

        public void Add(Match match)
        {
            Db.Matches.Add(match);
        }

        public void Add(Post post)
        {
            Db.Posts.Add(post);
        }

        public void Add(UserProfile profile)
        {
            Db.UserProfiles.Add(profile);
        }

        public void Delete(Board board)
        {
            Db.Boards.Remove(board);
        }

        public void Delete(Competitor competitor)
        {
            Db.Competitors.Remove(competitor);
        }

        public void Delete(Match match)
        {
            Db.Matches.Remove(match);
        }

        public void Delete(Post post)
        {
            Db.Posts.Remove(post);
        }

        public void Dispose()
        {
            Db.Dispose();
        }

        public Board GetBoardById(int id)
        {
            return (Boards.FirstOrDefault(x => x.BoardId == id));
        }

        public Board GetBoardByIdWithCompetitors(int id, bool includeProfiles = true)
        {
            if(includeProfiles)
                return (Boards.Include(c => c.Competitors.Select(p => p.Profile)).FirstOrDefault(x => x.BoardId == id));

            return (Boards.Include(c => c.Competitors).FirstOrDefault(x => x.BoardId == id));
        }

        public IQueryable<Board> GetBoardsForCompetitor(string userName)
        {
            var profileUserId = UserProfiles.FindProfile(userName).UserId;
            return
                (Boards.Where(
                    x => x.Competitors.Any(y => y.ProfileUserId == profileUserId)));
        }

        public Competitor GetCompetitorByName(int boardId, string name)
        {
            return
                (GetBoardByIdWithCompetitors(boardId)
                    .Competitors.FirstOrDefault(
                        x => x.Name.Equals(name, System.StringComparison.InvariantCultureIgnoreCase)));
        }

        public Competitor GetCompetitorById(int id)
        {
            return(Competitors.FirstOrDefault(x => x.CompetitorId.Equals(id)));
        }

        public Competitor GetCompetitorByUserName(int boardId, string userName)
        {
            var profile =
                UserProfiles.FirstOrDefault(
                    x => x.UserName.Equals(userName, System.StringComparison.InvariantCultureIgnoreCase));

            return
                (GetBoardByIdWithCompetitors(boardId)
                    .Competitors.FirstOrDefault(x => profile != null && x.ProfileUserId.Equals(profile.UserId)));
        }

        public Post GetPostById(int postId)
        {
            return (Posts.FirstOrDefault(p => p.PostId == postId));
        }

        public IQueryable<Match> GetResolvedMatchesByBoardId(int boardId, bool includeProfiles = true)
        {
            var matches = Matches.Where(x => x.Board.BoardId == boardId && x.Resolved.HasValue);

            return includeProfiles ? (matches.Include(l => l.Loser.Profile).Include(w => w.Winner.Profile)) : (matches);
        }

        public IQueryable<Match> GetUnresolvedMatchesByBoardId(int boardId, bool includeProfiles = true)
        {
            var matches = Matches.Where(x => x.Board.BoardId == boardId && !x.Resolved.HasValue);

            return includeProfiles ? (matches.Include(l => l.Loser.Profile).Include(w => w.Winner.Profile)) : (matches);
        }

        public Match GetMatchById(int id)
        {
            return (Matches.FirstOrDefault(x => x.MatchId == id));
        }

        public void CommitChanges()
        {
            CommitChanges(false);
        }

        public void CommitChanges(bool disableValidation)
        {
            try
            {
                if(disableValidation)
                    Db.Configuration.ValidateOnSaveEnabled = false;

                Db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
        }
    }
}