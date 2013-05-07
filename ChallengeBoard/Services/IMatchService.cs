using System.Collections.Generic;
using ChallengeBoard.Models;

namespace ChallengeBoard.Services
{
    public interface IMatchService
    {
        Match CreateMatch(int boardId, string winnerName, string loserName, string winnerComment = "", bool tie = false);
        Match GenerateMatch(int boardId, string winnerName, string loserName, bool tie = false);
        
        CompetitorStats CalculateCompetitorStats(Competitor competitor, ICollection<Match> matches);
        
        void RejectMatch(int boardId, int matchId, string userName);
        void VerifyMatch(int matchId);
        void SweepMatches();
    }
}