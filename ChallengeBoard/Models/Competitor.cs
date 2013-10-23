using System;
using System.ComponentModel.DataAnnotations;

namespace ChallengeBoard.Models
{
    public enum CompetitorStatus
    {
        Active = 0, 
        Retired = 1, 
        Banned = 2
    }
    public class Competitor
    {
        [Key]
        public int CompetitorId { get; set; }

        public UserProfile Profile { get; set; }
        public int ProfileUserId { get; set; }

        public int LastViewedPostId { get; set; }

        // TODO: Allow them to customize this on a board by board basis
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Status")]
        public CompetitorStatus Status { get; set; }

        [Display(Name = "Rating")]
        public int Rating { get; set; }

        [Display(Name = "Joined")]
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)]
        public DateTime Joined { get; set; }

        [Display(Name = "Wins")]
        public int Wins { get; set; }

        [Display(Name = "Losses")]
        public int Loses { get; set; }

        [Display(Name = "Ties")]
        public int Ties { get; set; }

        [Display(Name = "Streak")]
        public int Streak { get; set; }

        [Display(Name = "Matches")]
        public int MatchesPlayed
        {
            get { return (Wins + Loses + Ties); }
        }

        [Display(Name = "Win %")]
        [DisplayFormat(DataFormatString = "{0:0.0}")]
        public double WinPercentage
        {
            get { return Wins == 0 ? 0 : (Math.Round((double) Wins/MatchesPlayed*100, 1)); }
        }

        [Display(Name = "Rejections Received")]
        public int RejectionsReceived { get; set; }

        [Display(Name = "Rejections")]
        public int RejectionsGiven { get; set; }

        public Competitor()
        {
            Joined = DateTime.Now;
        }
    }
}