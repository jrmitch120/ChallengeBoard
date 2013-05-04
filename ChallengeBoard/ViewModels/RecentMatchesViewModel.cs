using System.Collections.Generic;
using ChallengeBoard.Models;

namespace ChallengeBoard.ViewModels
{
    public class RecentMatchesViewModel
    {
        public Board Board { get; set; }

        public IEnumerable<Match> Verified { get; set; }
        public IEnumerable<Match> UnVerified { get; set; }

        public Competitor Viewer { get; set; }

        public RecentMatchesViewModel()
        {
        }
    }
}