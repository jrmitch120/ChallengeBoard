using System;
using System.Linq;
using ChallengeBoard.Infrastucture;
using ChallengeBoard.Scoring;
using ChallengeBoard.Models;

namespace ChallengeBoard.Services
{
    public class MatchService : IMatchService
    {
        private readonly IRepository _repository;
        
        public MatchService(IRepository repository)
        {
            _repository = repository;
        }

        public Match CreateMatch(int boardId, string winnerName, string loserName, bool tie = false)
        {
            var match = GenerateMatch(boardId, winnerName, loserName, tie);

            _repository.Add(match);
            _repository.CommitChanges();

            return (match);
        }

        public Match GenerateMatch(int boardId, string winnerName, string loserName, bool tie = false)
        {
            var board = _repository.GetBoardByIdWithCompetitors(boardId);

            if(board == null)
                throw (new ServiceException("Can not find challenge board."));
            if (DateTime.Now >= board.End)
                throw (new ServiceException("This challenge board has ended."));

            var winner = board.Competitors.Active().FindCompetitor(winnerName);
            var loser = board.Competitors.Active().FindCompetitor(loserName);
            
            if(winner == null)
                throw (new ServiceException("You are not part of this challenge board."));
            if (loser == null)
                throw (new ServiceException("Can not find opponent."));
            if (loser.Name == winner.Name)
                throw (new ServiceException("You can't play yourself."));

            var match = new Match
            {
                Board = board,
                Tied = tie,
                Winner = winner,
                Loser = loser,
                Created = DateTime.Now,
                VerificationDeadline = DateTime.Now.AddHours(board.AutoVerification)
            };

            var unresolvedMatches = _repository.GetUnresolvedMatchesByBoardId(boardId).ToList();

            // Run scoring calculation
            IScoringSystem system = new StandardElo();
            var eloResult = system.Calculate(winner.CalculateUnverifiedRank(unresolvedMatches),
                                             loser.CalculateUnverifiedRank(unresolvedMatches), tie);

            match.WinnerRatingDelta = eloResult.WinnerDelta.RoundToWhole();
            match.LoserRatingDelta = eloResult.LoserDelta.RoundToWhole();

            return (match);
        }

        public void RejectMatch(int boardId, int matchId, string userName)
        {
            // Grab user profile
            var user = _repository.UserProfiles.FindProfile(userName);
            
            // All unresolved matches for this challenge board.
            var unresolvedMatches =
                _repository.GetUnresolvedMatchesByBoardId(boardId).OrderBy(x => x.VerificationDeadline).ToList();

            var rejectedMatch = unresolvedMatches.SingleOrDefault(x => x.MatchId == matchId);

            if (rejectedMatch == null)
                throw new ServiceException("Can not find match.");
            
            if (rejectedMatch.Loser.ProfileUserId != user.UserId)
                throw new ServiceException("You are not able to reject this match.");

            rejectedMatch.Rejected = true;
            rejectedMatch.Resolved = DateTime.Now;
            rejectedMatch.Winner.RejectionsReceived++;
            rejectedMatch.Loser.RejectionsGiven++;

            // Anonymous list of unresolve matches taking place after the rejected match.
            // * These are the matches we need to recalculate
            var matchList =
                unresolvedMatches.Select(
                    x => new {x.MatchId, x.Created})
                                 .Where(x => x.Created >= rejectedMatch.Created && x.MatchId != rejectedMatch.MatchId);

            IScoringSystem system = new StandardElo();
            
            foreach (var match in matchList)
            {
                // Get unresolved matches prior to this one
                var filteredUnresolved =
                    unresolvedMatches.Where(x => x.Created <= match.Created && x.MatchId != match.MatchId).ToList();

                // Pick out the match to recalc and save
                var matchToRecalc = unresolvedMatches.First(x => x.MatchId == match.MatchId);

                // Run the recalc
                var eloRecalc = system.Calculate(matchToRecalc.Winner.CalculateUnverifiedRank(filteredUnresolved),
                                                 matchToRecalc.Loser.CalculateUnverifiedRank(filteredUnresolved),
                                                 matchToRecalc.Tied);

                // Update the ratings
                matchToRecalc.WinnerRatingDelta = eloRecalc.WinnerDelta.RoundToWhole();
                matchToRecalc.LoserRatingDelta = eloRecalc.LoserDelta.RoundToWhole();
            }

            _repository.CommitChanges();
        }

        public void SweepMatches()
        {
            //var matches = _repository.Matches.Where(m => !m.Verified && !m.Rejected);
            var matches = _repository.Matches.Where(m => m.VerificationDeadline < DateTime.Now && !m.Verified && !m.Rejected);

            foreach (var match in matches)
                VerifyMatch(match, false);

            _repository.CommitChanges();
        }

        public void VerifyMatch(int matchId)
        {
            var match = _repository.GetMatchById(matchId);

            if (match == null)
                throw (new ServiceException("Match not found."));

            VerifyMatch(match);
        }

        private void VerifyMatch(Match match, bool commit=true)
        {
            match.Winner.Rating += match.WinnerRatingDelta;
            match.Loser.Rating += match.LoserRatingDelta;
            
            if (match.Tied)
            {
                match.Winner.Ties += 1;
                match.Loser.Ties += 1;
                match.Winner.Streak = match.Loser.Streak = 0;
            }
            else
            {
                match.Loser.Loses += 1;
                match.Loser.Streak = 0;

                match.Winner.Wins += 1;
                match.Winner.Streak += 1;
            }
            
            match.Verified = true;
            match.Resolved = DateTime.Now;

            if (commit)
                _repository.CommitChanges();
        }
    }
}