using ChallengeBoard.Models;
using PagedList;

namespace ChallengeBoard.ViewModels
{
    public class StandingsViewModel
    {
        public Board Board { get; set; }
        public IPagedList<Competitor> Standings { get; set; }
        public bool Unofficial { get; set; }
    }
}