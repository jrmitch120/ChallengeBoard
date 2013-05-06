using System;
using System.Linq;

namespace ChallengeBoard.Models
{
    public static class RepositoryExtensions
    {
        // Profile
        public static UserProfile FindProfile(this IQueryable<UserProfile> profiles, string name)
        {
            return (profiles.FirstOrDefault(x => x.UserName.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
        }

        // Match
        public static IQueryable<Match> InvolvesCompetitor(this IQueryable<Match> matches, Competitor competitor)
        {
            return
                (matches.Where(
                    x =>
                    x.Winner.CompetitorId == competitor.CompetitorId || x.Loser.CompetitorId == competitor.CompetitorId));
        }
    }
}