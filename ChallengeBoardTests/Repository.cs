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

            var competitor1 = new Competitor
            {
                Name = "User1",
                ProfileUserId = 1,
                Profile = new UserProfile
                {
                    UserId = 1,
                    UserName = "User1"
                },
                Rating = 1500,
            };

            var competitor2 = new Competitor
            {
                Name = "User2",
                ProfileUserId = 1,
                Profile = new UserProfile
                {
                    UserId = 1,
                    UserName = "User2"
                },
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

            board.Matches = new[] { match1, match2, match3 };

            repository.Add(board);
            repository.Add(competitor1);
            repository.Add(competitor2);
            repository.Add(match1);
            repository.Add(match2);
            repository.Add(match3);

            return (repository);
        }
    }
}
