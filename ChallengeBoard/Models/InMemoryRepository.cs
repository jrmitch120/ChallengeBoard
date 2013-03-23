using System.Collections.Generic;
using System.Linq;

namespace ChallengeBoard.Models
{
    public class InMemoryRepository : IRepository
    {
        private readonly ICollection<Board> _boards;
        private readonly ICollection<Competitor> _competitors;
        private readonly ICollection<Match> _matches;
        private readonly ICollection<UserProfile> _profiles;

        public InMemoryRepository()
        {
            _boards = new List<Board>();
            _competitors = new List<Competitor>();
            _matches = new List<Match>();
            _profiles = new List<UserProfile>();
        }

        public IQueryable<Board> Boards
        {
            get { return (_boards.AsQueryable()); }
        }

        public IQueryable<Competitor> Competitors
        {
            get { return (_competitors.AsQueryable()); }
        }

        public IQueryable<Match> Matches
        {
            get { return (_matches.AsQueryable()); }
        }

        public IQueryable<UserProfile> UserProfiles
        {
            get { return (_profiles.AsQueryable()); }
            
        }

        public void Add(Board board)
        {
            _boards.Add(board);
        }

        public void Add(Competitor competitor)
        {
            _competitors.Add(competitor);
        }

        public void Add(Match match)
        {
            _matches.Add(match);
        }

        public void Add(UserProfile profile)
        {
            _profiles.Add(profile);
        }

        public void Delete(Board board)
        {
            _boards.Remove(board);
        }

        public void Delete(Competitor competitor)
        {
            _competitors.Remove(competitor);
        }

        public void Delete(Match match)
        {
            _matches.Remove(match);
        }

        public void Dispose()
        {
           
        }

        public Board GetBoardById(int id)
        {
            return (Boards.FirstOrDefault(x => x.BoardId == id));
        }

        public Board GetBoardByIdWithCompetitors(int id, bool includeProfiles = true)
        {
            return (Boards.FirstOrDefault(x => x.BoardId == id));
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

        public IQueryable<Match> GetUnresolvedMatchesByBoardId(int id)
        {
            return (Matches.Where(x => !x.Resolved.HasValue));
        }

        public Match GetMatchById(int id)
        {
            return (Matches.FirstOrDefault(x => x.MatchId == id));
        }

        public void CommitChanges()
        {
        }
    }
}