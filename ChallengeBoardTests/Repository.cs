using System;
using System.Collections.Generic;
using ChallengeBoard.Models;

namespace ChallengeBoardTests
{
    public static class Repository
    {
        public static IRepository CreatePopulatedRepository()
        {
            var repository = new InMemoryRepository();

            var competitorProfile1 = new UserProfile
            {
                UserId = 1,
                UserName = "User1"
            };

            var competitorProfile2 = new UserProfile
            {
                UserId = 2,
                UserName = "User2"
            };

            var competitor1 = new Competitor
            {
                Name = "User1",
                ProfileUserId = 1,
                Profile = competitorProfile1,
                Rating = 1500,
            };

            var competitor2 = new Competitor
            {
                Name = "User2",
                ProfileUserId = 2,
                Profile = competitorProfile2,
                Rating = 1500,
            };

            var board = new Board
            {
                BoardId = 1,
                AutoVerification = 1,
                Created = DateTime.Now,
                Description = "Test Board",
                End = DateTime.Now.AddDays(30),
                Name = "Test Board",
                Owner = new Competitor {Name = "User1"},
                Started = DateTime.Now,
                StartingRating = 1500,
                Competitors = new [] {competitor1, competitor2}
            };

            // Unresolved match
            var match1 = new Match
            {
                Board = board,
                Created = DateTime.Now,
                Loser = competitor2,
                Winner = competitor1,
                VerificationDeadline = DateTime.Now.AddHours(board.AutoVerification),
                MatchId = 1,
                LoserRatingDelta = -10,
                WinnerRatingDelta = 10
            };

            // Unresolved match
            var match2 = new Match
            {
                Board = board,
                Created = DateTime.Now,
                Loser = competitor1,
                Winner = competitor2,
                VerificationDeadline = DateTime.Now.AddHours(board.AutoVerification),
                MatchId = 2,
                LoserRatingDelta = -10,
                WinnerRatingDelta = 10
            };

            // Unresolved match
            var match3 = new Match
            {
                Board = board,
                Created = DateTime.Now,
                Loser = competitor2,
                Winner = competitor1,
                VerificationDeadline = DateTime.Now.AddHours(board.AutoVerification),
                MatchId = 3,
                LoserRatingDelta = -10,
                WinnerRatingDelta = 10
            };

            // Resolved Match [Verified]
            var match4 = new Match
            {
                Board = board,
                Created = DateTime.Now.AddDays(-7),
                Loser = competitor1,
                Winner = competitor2,
                VerificationDeadline = DateTime.Now.AddHours(board.AutoVerification),
                MatchId = 4,
                LoserRatingDelta = -10,
                WinnerRatingDelta = 10,
                Resolved = DateTime.Now.AddHours(board.AutoVerification),
                Verified = true
            };

            // Resolved Match [Rejected]
            var match5 = new Match
            {
                Board = board,
                Created = DateTime.Now.AddDays(-7),
                Loser = competitor2,
                Winner = competitor1,
                VerificationDeadline = DateTime.Now.AddHours(board.AutoVerification),
                MatchId = 4,
                LoserRatingDelta = -10,
                WinnerRatingDelta = 10,
                Resolved = DateTime.Now.AddHours(board.AutoVerification),
                Rejected = true
            };

            board.Matches = new[] { match1, match2, match3 };

            repository.Add(board);
            repository.Add(competitorProfile1);
            repository.Add(competitorProfile2);
            repository.Add(competitor1);
            repository.Add(competitor2);
            repository.Add(match1);
            repository.Add(match2);
            repository.Add(match3);
            repository.Add(match4);
            repository.Add(match5);

            return (repository);
        }
    }
}
