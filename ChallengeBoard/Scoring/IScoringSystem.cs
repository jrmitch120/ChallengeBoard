namespace ChallengeBoard.Scoring
{
    public interface IScoringSystem
    {
        ScoringResult Calculate(double boardStartingRating, double winnerRating, double loserRating, bool tie = false);
    }
}