using System;
using System.ComponentModel.DataAnnotations;
using ChallengeBoard.Models;

namespace ChallengeBoard.ViewModels
{
    public class CompetitorViewModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }

        public CompetitorViewModel(Competitor competitor)
        {
            Name = competitor.Name;
        }
    }
}