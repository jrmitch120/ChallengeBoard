using System;
using System.Collections.Generic;
using System.Linq;

namespace ChallengeBoard.Models
{
    public static class RepositoryExtensions
    {
        public static UserProfile FindProfile(this IQueryable<UserProfile> profiles, string name)
        {
            return (profiles.FirstOrDefault(x => x.UserName.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}