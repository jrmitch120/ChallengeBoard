using System;
using System.ComponentModel.DataAnnotations;

namespace ChallengeBoard.Models
{
    public class Match
    {
        [Key]
        public int MatchId { get; set; }

        public Board Board { get; set; }
        //public int BoardId { get; set; }

        public int WinnerRatingDelta { get; set; }
        public int LoserRatingDelta { get; set; }

        [Display(Name = "Reporter")]
        public Competitor Winner { get; set; }

        [Display(Name = "Opponent")]
        public Competitor Loser { get; set; }

        [Display(Name = "Comments")]
        [MaxLength(140)]
        public string WinnerComment { get; set; }

        [Display(Name = "Comments")]
        [MaxLength(140)]
        public string LoserComment { get; set; }

        [Display(Name = "Reported On")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime Created { get; set; }
        public DateTime VerificationDeadline { get; set; }

        [DisplayFormat(DataFormatString = "{0:g}")]
        [Display(Name = "Resolved On")]
        public DateTime? Resolved { get; set; }

        public bool Tied { get; set; }
        public bool Verified { get; set; }
        public bool Rejected { get; set; }
        public bool IsResolved { get { return (Rejected || Verified); } }
    }
}