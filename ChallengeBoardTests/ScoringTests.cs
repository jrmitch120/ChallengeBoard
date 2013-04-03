using ChallengeBoard.Scoring;
using NUnit.Framework;

namespace ChallengeBoardTests
{
    [TestFixture]
    public class ScoringTests
    {
        // http://www.3dkingdoms.com/chess/elo.htm

        [Test]
        public void StandardElo()
        {
            var standardElo = new StandardElo();

            // Standard match
            var result = standardElo.Calculate(1600, 1500, 1500);

            Assert.That(result.WinnerDelta, Is.EqualTo(16));
            Assert.That(result.LoserDelta, Is.EqualTo(-16));

            // K32 vs K24
            result = standardElo.Calculate(1600, 1500, 2300);

            Assert.That(result.WinnerDelta, Is.EqualTo(32));
            Assert.That(result.LoserDelta, Is.EqualTo(-24));

            // K32 vs K16
            result = standardElo.Calculate(1600, 1600, 2402);

            Assert.That(result.WinnerDelta, Is.EqualTo(32));
            Assert.That(result.LoserDelta, Is.EqualTo(-16));

            // K24 vs K16
            result = standardElo.Calculate(1600, 2100, 2400);

            Assert.That(result.WinnerDelta, Is.EqualTo(20));
            Assert.That(result.LoserDelta, Is.EqualTo(-14));

            // K32 Tie
            result = standardElo.Calculate(1600, 1600, 1800, true);

            Assert.That(result.WinnerDelta, Is.EqualTo(8));
            Assert.That(result.LoserDelta, Is.EqualTo(-8));
        }
    }
}
