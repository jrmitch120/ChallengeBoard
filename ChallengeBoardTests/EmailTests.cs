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
            const string winnerComment = "X_WINNER_COMMENT_X";
            const string winnerName = "X_WINNER_NAME_X";

            var message = EmailFactory.ParseTemplate(new MatchNotification
            {
                AutoVerifies = autoVerified,
                BoardName = boardName,
                LoserName = loserName,
                WinnerComment = winnerComment,
                WinnerName = winnerName

            }, EmailType.MatchNotification);

            Assert.That(message.Contains(autoVerified.ToString()));
            Assert.That(message.Contains(boardName));
            Assert.That(message.Contains(loserName));
            Assert.That(message.Contains(winnerComment));
            Assert.That(message.Contains(winnerName));
        }

        [Test]
        public void ParseRejectionTemplate()
        {
            const string boardName = "X_BOARDNAME_X";
            const string boardOwnerName = "X_OWNERNAME_X";
            const string loserName = "X_LOSERNAME_X";
            const string winnerName = "X_WINNER_NAME";

            var message = EmailFactory.ParseTemplate(new MatchRejectionNotice
            {
                BoardName = boardName,
                BoardOwnerName = boardOwnerName,
                RejectedName = loserName,
                RejectorName = winnerName

            }, EmailType.MatchRejectionNotice);

            Assert.That(message.Contains(boardName));
            Assert.That(message.Contains(boardOwnerName));
            Assert.That(message.Contains(loserName));
            Assert.That(message.Contains(winnerName));
        }

        [Test]
        public void ParsePasswordRecoveryTemplate()
        {
            const string userName = "X_USERNAME_X";
            const string recoveryLink = "X_RECOVERYLINK_X";

            var message = EmailFactory.ParseTemplate(new PasswordRecovery
            {
                UserName = userName,
                RecoveryLink = recoveryLink
            }, EmailType.PasswordRecovery);

            Assert.That(message.Contains(userName));
            Assert.That(message.Contains(recoveryLink));
        }
    }
}
