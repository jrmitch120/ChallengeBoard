using System.Collections.Generic;
using ChallengeBoard.Models;

namespace ChallengeBoard.ViewModels
{
    public class MatchesViewModel
    {
        public IEnumerable<Match> Matches { get; set; }
        public Competitor Viewer { get; set; }
    }
}