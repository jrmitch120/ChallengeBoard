using System;
using System.Collections.Generic;
using System.Linq;

namespace ChallengeBoard.Models
{
    public static class ModelExtensions
    {
        public static IEnumerable<Competitor> Active(this ICollection<Competitor> competitors)
        {
            return (competitors.Where(x => x.Status == CompetitorStatus.Active));
        }

        public static IEnumerable<Competitor> Retired(this ICollection<Competitor> competitors)
        {
            return (competitors.Where(x => x.Status == CompetitorStatus.Retired));
        }

        public static bool ContainsCompetitor(this IEnumerable<Competitor> competitors, string name)
        {
            return (competitors.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
        }

        public static Competitor FindCompetitor(this IEnumerable<Competitor> competitors, string name)
        {
            return (competitors.SingleOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
        }

        public static int CalculateUnverifiedRank(this Competitor competitor , IList<Match> matches)
        {
            return (competitor.Rating +
                    matches.Where(x => x.Loser.ProfileUserId == competitor.ProfileUserId && !x.IsResolved)
                           .Sum(x => x.LoserRatingDelta) +
                    matches.Where(x => x.Winner.ProfileUserId == competitor.ProfileUserId && !x.IsResolved)
                           .Sum(x => x.WinnerRatingDelta));
        }
    }
}