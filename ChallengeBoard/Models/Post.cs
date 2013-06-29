using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ChallengeBoard.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        public Board Board { get; set; }

        public Competitor Owner { get; set; }

        [AllowHtml]
        [Required]
        public string Body { get; set; }

        [Display(Name = "Created")]
        [DisplayFormat(DataFormatString = "{0:M/d/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Created { get; set; }

        [Display(Name = "Last Edited")]
        [DisplayFormat(DataFormatString = "{0:M/d/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Edited { get; set; }
        
        public Post()
        {
            Created = DateTime.Now;
        }
    }
}