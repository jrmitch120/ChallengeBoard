using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChallengeBoard.Models
{
    [NotMapped]
    public class CompetitorStats
    {
        public readonly ICollection<PvpStats> Pvp;// IDictionary<Competitor, PvpStats> Pvp;

        public CompetitorStats()
        {
            // TODO Summed match PVP Stat calcs
            Pvp = new List<PvpStats>();
        }
    }

    [NotMapped]
    public class PvpStats
    {
        public Competitor Opponent { get; set; }

        [Display(Name = "Wins")]
        public int Wins { get; set; }

        [Display(Name = "Losses")]
        public int Loses { get; set; }

        [Display(Name = "Ties")]
        public int Ties { get; set; }

        [Display(Name = "Net Rank")]
        public int EloNet { get; set; }

        [Display(Name = "Matches")]
        public int MatchesPlayed
        {
            get { return (Wins + Loses + Ties); }
        }

        [Display(Name = "Win %")]
        [DisplayFormat(DataFormatString = "{0:0.0}")]
        public double WinPercentage
        {
            get { return Wins == 0 ? 0 : (Math.Round((double)Wins / MatchesPlayed * 100, 1)); }
        }
    }
}