using System;
using System.Linq;

namespace ChallengeBoard.Models
{
    public interface IRepository : IDisposable
    {
        void CommitChanges();
        void CommitChanges(bool disableValidation);
        
        Board GetBoardById(int id);
        Match GetMatchById(int id);

        IQueryable<UserProfile> UserProfiles { get; }
        IQueryable<Competitor> Competitors { get; }
        IQueryable<Board> Boards { get; }
        IQueryable<Match> Matches { get; }

        IQueryable<Board> GetBoardsForCompetitor(string userName);
        IQueryable<Match> GetUnresolvedMatchesByBoardId(int boardId, bool includeProfiles = true);
        IQueryable<Match> GetResolvedMatchesByBoardId(int boardId, bool includeProfiles = true);

        void Add(Competitor competitor);
        void Add(Board board);
        void Add(Match match);
        void Add(UserProfile profile);

        void Delete(Board board);
        void Delete(Competitor competitor);
        void Delete(Match match);

        Board GetBoardByIdWithCompetitors(int id, bool includeProfiles = true);
        Competitor GetCompetitorById(int boardId, int competitorId);
        Competitor GetCompetitorByName(int boardId, string name);
    }
}