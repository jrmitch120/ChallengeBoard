using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ChallengeBoard.Models
{
    public class Board
    {
        [Key]
        public int BoardId { get; set; }

        public Competitor Owner { get; set; }

        public ICollection<Competitor> Competitors { get; set; }
        public ICollection<Match> Matches { get; set; }

        [Required]
        [MaxLength(45)]
        [Display(Name = "Board Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [AllowHtml]
        public string Description { get; set; }

        [Display(Name = "Password")]
        [MaxLength(32)]
        public string Password { get; set; }

        [Display(Name = "Created")]
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)]
        public DateTime Created { get; set; }

        [Display(Name = "Starting Rating")]
        [Range(1200,1800)]
        public int StartingRating { get; set; }

        [Display(Name = "Auto-Verifies In (hrs)")]
        [Range(1, 336)] // 2 Week upper limit
        public int AutoVerification { get; set; }

        [Display(Name = "Ends")]
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)]
        [CustomValidation(typeof(Board), "ValidateEndDate")]
        public DateTime End { get; set; }

        [Display(Name = "Started")]
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)]
        public DateTime Started { get; set; }

        public static ValidationResult ValidateEndDate(DateTime date)
        {
            if (date.Date < DateTime.Today)
                return new ValidationResult("Ending date cannot be in the past.");

            if (date.Date > DateTime.Today.AddYears(1))
                return new ValidationResult("Ending date must be within the next year.");

            return ValidationResult.Success;
        }

        public Board()
        {
            Competitors = new List<Competitor>();
            StartingRating = 1500;
            AutoVerification = 72;
            Created = DateTime.Now;
            End = DateTime.Now.AddMonths(1);
        }
    }
}
