namespace ChallengeBoard.Scoring
{
    public interface IScoringSystem
    {
        ScoringResult Calculate(double winnerRating, double loserRating, bool tie = false);
    }
}