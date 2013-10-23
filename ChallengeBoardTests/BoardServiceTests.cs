using System;
using System.Linq;
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

            var boardService = new BoardService(repository);
            
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

            var boardService = new BoardService(repository);

            boardService.AdjustMatchDeadlines(board);

            foreach (var match in board.Matches)
                Assert.That(match.VerificationDeadline, Is.EqualTo(match.Created.AddHours(10)));
        }

        [Test]
        public void PostsToReadMeta()
        {
            var repository = Repository.CreatePopulatedRepository();
            var boardService = new BoardService(repository);

            var competitor = repository.GetBoardByIdWithCompetitors(1).Competitors.First();

            var meta = boardService.GetDiscussionMeta(1, competitor.Name);

            Assert.That(meta.NumberOfNewPosts, Is.GreaterThan(0));
            Assert.That(meta.NewPosts, Is.True);
        }

        [Test]
        public void PostsAllReadMeta()
        {
            var repository = Repository.CreatePopulatedRepository();
            var boardService = new BoardService(repository);

            var competitor = repository.GetBoardByIdWithCompetitors(1).Competitors.First();

            competitor.LastViewedPostId = repository.Posts.Where(p => p.Board.BoardId == 1).Max(p => p.PostId);

            var meta = boardService.GetDiscussionMeta(1, competitor.Name);

            Assert.That(meta.NumberOfNewPosts, Is.EqualTo(0));
            Assert.That(meta.NewPosts, Is.False);
        }
    }
}
