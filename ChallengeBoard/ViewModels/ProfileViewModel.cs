using System.Collections.Generic;
using ChallengeBoard.Models;

namespace ChallengeBoard.ViewModels
{
    public class ProfileViewModel
    {
        public int BoardId { get; set; }
        public Competitor Competitor { get; set; }
        public IEnumerable<Match> Matches { get; set; }
    }
}