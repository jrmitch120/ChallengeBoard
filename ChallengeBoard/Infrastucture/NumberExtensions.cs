using System;

namespace ChallengeBoard.Infrastucture
{
    public static class NumberExtensions
    {
        /// <summary>
        /// Round double to nearest whole number
        /// </summary>
        public static int RoundToWhole(this double d)
        {
            return (int)Math.Round(d, 0, MidpointRounding.AwayFromZero);
        }
    } 
}