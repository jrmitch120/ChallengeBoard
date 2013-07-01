using System.ComponentModel.DataAnnotations;
using ChallengeBoard.Models;

namespace ChallengeBoard.ViewModels
{
    public class CompetitorViewModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }

        public int CompetitorId { get; set; }

        public CompetitorViewModel() {}

        public CompetitorViewModel(Competitor competitor)
        {
            Name = competitor.Name;
            CompetitorId = competitor.CompetitorId;
        }
    }
}