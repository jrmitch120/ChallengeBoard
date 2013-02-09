using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using ChallengeBoard.Services;

namespace ChallengeBoardTests
{
    [TestFixture]
    public class GenerateMatchTests
    {
        [Test]
        public void ThrowsIfBoardDoesNotExist()
        {
            var repository = Repository.CreatePopulatedRepository();
            var service = new MatchService(repository, null);

            Assert.Throws<ServiceException>(() => service.GenerateMatch(100, "User1", "User3"));
        }

        [Test]
        public void ThrowsIfReporterNotFound()
        {
            var repository = Repository.CreatePopulatedRepository();
            var service = new MatchService(repository, null);

            Assert.Throws<ServiceException>(() => service.GenerateMatch(1, "User3", "User2"));
        }

        [Test]
        public void ThrowsIfOpponentNotFound()
        {
            var repository = Repository.CreatePopulatedRepository();
            var service = new MatchService(repository, null);
            
            Assert.Throws<ServiceException>(() => service.GenerateMatch(1, "User1", "User3"));
        }

        [Test]
        public void ThrowsIfPlayingYourself()
        {
            var repository = Repository.CreatePopulatedRepository();
            var service = new MatchService(repository, null);

            // Can't find board
            Assert.Throws<ServiceException>(() => service.GenerateMatch(1, "User1", "User1"));
        }

        [Test]
        public void ThrowsIfBoardHasEnded()
        {
            var repository = Repository.CreatePopulatedRepository();
            var service = new MatchService(repository, null);

            var board = repository.GetBoardByIdWithCompetitors(1);
            board.End = DateTime.Now.AddDays(-1);

            Assert.Throws<ServiceException>(() => service.GenerateMatch(1, "User1", "User2"));
        }

        [Test]
        public void GeneratesMatch()
        {
            var repository = Repository.CreatePopulatedRepository();
            var service = new MatchService(repository, null);

            var match = service.GenerateMatch(1, "User1", "User2");

            Assert.NotNull(match);
        }
    }

    [TestFixture]
    public class CreateMatchTests
    {
        [Test]
        public void CreatesMatch()
        {
            var repository = Repository.CreatePopulatedRepository();
            
            var service = new MatchService(repository, new Mock<IMailService>().Object);

            var matchCount = repository.Matches.Count();

            service.CreateMatch(1, "User1", "User2");

            Assert.That(repository.Matches.Count(), Is.EqualTo(matchCount + 1));
        }
    }

    [TestFixture]
    public class MatchVerificationTests
    {
        [Test]
        public void ThrowsIfMatchNotFound()
        {
            var repository = Repository.CreatePopulatedRepository();
            var service = new MatchService(repository, null);

            Assert.Throws<ServiceException>(() => service.VerifyMatch(100));
        }
        
        [Test]
        public void VerifiesMatch()
        {
            var repository = Repository.CreatePopulatedRepository();
            var service = new MatchService(repository, null);
            
            service.VerifyMatch(1);
            
            var match = repository.GetMatchById(1);

            Assert.That(match.Verified, Is.True);
            Assert.That(match.Resolved, Is.Not.Null);

            Assert.That(match.Loser.Wins, Is.EqualTo(0));
            Assert.That(match.Loser.Streak, Is.EqualTo(0));
            Assert.That(match.Loser.Loses, Is.EqualTo(1));
            Assert.That(match.Loser.Ties, Is.EqualTo(0));

            Assert.That(match.Winner.Wins, Is.EqualTo(1));
            Assert.That(match.Winner.Streak, Is.EqualTo(1));
            Assert.That(match.Winner.Loses, Is.EqualTo(0));
            Assert.That(match.Winner.Ties, Is.EqualTo(0));
        }

        [Test]
        public void VerifiesTiedMatch()
        {
            var repository = Repository.CreatePopulatedRepository();
            var service = new MatchService(repository, null);

            repository.GetMatchById(1).Tied = true;
            
            service.VerifyMatch(1);

            var match = repository.GetMatchById(1);

            Assert.That(match.Verified, Is.True);
            Assert.That(match.Resolved, Is.Not.Null);

            Assert.That(match.Loser.Wins, Is.EqualTo(0));
            Assert.That(match.Loser.Streak, Is.EqualTo(0));
            Assert.That(match.Loser.Loses, Is.EqualTo(0));
            Assert.That(match.Loser.Ties, Is.EqualTo(1));

            Assert.That(match.Winner.Wins, Is.EqualTo(0));
            Assert.That(match.Winner.Streak, Is.EqualTo(0));
            Assert.That(match.Winner.Loses, Is.EqualTo(0));
            Assert.That(match.Winner.Ties, Is.EqualTo(1));
        }
    }

    [TestFixture]
    public class MatchRejectionTests
    {
        [Test]
        public void ThrowsIfMatchNotFound()
        {
            var repository = Repository.CreatePopulatedRepository();
            var service = new MatchService(repository, null);
            var match = repository.GetUnresolvedMatchesByBoardId(1).First();

            Assert.Throws<ServiceException>(() => service.RejectMatch(match.Board.BoardId, 9999, match.Loser.Name));
        }

        [Test]
        public void ThrowsIfNotLoser()
        {
            var repository = Repository.CreatePopulatedRepository();
            var service = new MatchService(repository, null);

            var match = repository.GetUnresolvedMatchesByBoardId(1).First();
            Assert.Throws<ServiceException>(() => service.RejectMatch(match.Board.BoardId, match.MatchId, match.Winner.Name));
        }

        [Test]
        public void ThrowsIfResolved()
        {
            var repository = Repository.CreatePopulatedRepository();
            var service = new MatchService(repository, null);

            var match = repository.Matches.First(x => x.Board.BoardId == 1 && x.Resolved.HasValue);
            Assert.Throws<ServiceException>(() => service.RejectMatch(match.Board.BoardId, match.MatchId, match.Loser.Name));
        }

        // Todo, continue
    }
}
