using System;

namespace ChallengeBoard.Scoring
{
    [ScoringSystem("Elo","Standard Elo scoring system")]
    public class StandardElo : IScoringSystem
    {
        // Rating Disparity. The higher is F, the easier it is to gain points (or to lose them)
        private const int F = 400;

        /// <summary>
        /// Class for calculating player ELO gain/loss
        /// </summary>
        public StandardElo(){}

        public ScoringResult Calculate(double winnerRating, double loserRating, bool tie = false)
        {
            //http://en.wikipedia.org/wiki/Elo_rating_system
            //http://www.chess-mind.com/en/elo-system

            int k;

            // Game Importance.
            if (winnerRating < 2100 || loserRating < 2100)
                k = KFactor.Low;
            else if (winnerRating < 2400 || loserRating < 2400)
                k = KFactor.Medium;
            else
                k = KFactor.High;

            var eW = 1 / (1 + Math.Pow(10,(loserRating - winnerRating)/F));
            var eL = 1 / (1 + Math.Pow(10,(winnerRating - loserRating)/F));

            var results = new ScoringResult();

            if (!tie)
            {
                results.WinnerDelta = Math.Round(k*(1 - eW));
                results.LoserDelta = Math.Round(k*(0 - eL));
            }
            else
            {
                results.WinnerDelta = Math.Round(k * (.5 - eW));
                results.LoserDelta = Math.Round(k * (.5 - eL));
            }

            return (results);
        }
    }
}