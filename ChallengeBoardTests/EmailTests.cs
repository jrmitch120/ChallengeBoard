using NUnit.Framework;
using ChallengeBoard.Email;
using ChallengeBoard.Email.Models;

namespace ChallengeBoardTests
{
    [TestFixture]
    public class EmailFactoryTests
    {
        [Test]
        public void ParseMatchTemplate()
        {
            const int autoVerified = 100;
            const string boardName = "X_BOARDNAME_X";
            const string loserName = "X_LOSERNAME_X";
            const string winnerName = "X_WINNER_NAME";

            var message = EmailFactory.ParseTemplate(new MatchNotification
            {
                AutoVerifies = autoVerified,
                BoardName = boardName,
                LoserName = loserName,
                WinnerName = winnerName

            }, EmailType.MatchNotification);

            Assert.That(message.Contains(autoVerified.ToString()));
            Assert.That(message.Contains(boardName));
            Assert.That(message.Contains(loserName));
            Assert.That(message.Contains(winnerName));
        }
    }
}
