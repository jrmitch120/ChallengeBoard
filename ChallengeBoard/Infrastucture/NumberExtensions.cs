namespace ChallengeBoard.Infrastucture
{
    public static class NumberExtensions
    {
        /// <summary>
        /// Round double to nearest whole number
        /// </summary>
        public static int RoundToWhole(this double d)
        {
            return (int)(d + 0.5);
        }
    } 
}