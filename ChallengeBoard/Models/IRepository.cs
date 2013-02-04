using System;
using System.Linq;

namespace ChallengeBoard.Models
{
    public interface IRepository : IDisposable
    {
        void CommitChanges();
        
        Board GetBoardById(int id);
        Match GetMatchById(int id);

        IQueryable<UserProfile> UserProfiles { get; }
        IQueryable<Competitor> Competitors { get; }
        IQueryable<Board> Boards { get; }
        IQueryable<Match> Matches { get; }

        IQueryable<Match> GetUnresolvedMatchesByBoardId(int id);

        void Add(Competitor competitor);
        void Add(Board board);
        void Add(Match match);
        void Add(UserProfile profile);

        void Delete(Board board);
        void Delete(Competitor competitor);
        void Delete(Match match);

        Board GetBoardByIdWithCompetitors(int id);
    }
}