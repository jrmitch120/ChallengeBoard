using System;
using ChallengeBoard.Models;
using ChallengeBoard.Services;
using NUnit.Framework;

namespace ChallengeBoardTests
{
    [TestFixture]
    public class BoardServiceTests
    {
        [Test]
        public void AdjustMatchesForward()
        {
            var repository = Repository.CreatePopulatedRepository();
            var board = repository.GetBoardById(1);
            
            board.AutoVerification = 10;

            var boardService = new BoardService();
            
            boardService.AdjustMatchDeadlines(board);

            foreach (var match in board.Matches)
                Assert.That(match.VerificationDeadline, Is.EqualTo(match.Created.AddHours(10)));
        }

        [Test]
        public void AdjustMatchesBackward()
        {
            var repository = Repository.CreatePopulatedRepository();
            var board = repository.GetBoardById(1);

            board.AutoVerification = 10;

            foreach (var match in board.Matches)
                match.VerificationDeadline = DateTime.Now.AddDays(3);

            var boardService = new BoardService();

            boardService.AdjustMatchDeadlines(board);

            foreach (var match in board.Matches)
                Assert.That(match.VerificationDeadline, Is.EqualTo(match.Created.AddHours(10)));
        }
    }
}
