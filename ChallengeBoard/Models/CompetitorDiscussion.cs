using System.ComponentModel.DataAnnotations.Schema;

namespace ChallengeBoard.Models
{
    [NotMapped]
    public class CompetitorDiscussion
    {
        public int LatestPostId { get; set; }
        public int LastViewedPostId { get; set; }
        
        public int NumberOfNewPosts { get; set; }

        public bool NewPosts 
        {
            get { return (NumberOfNewPosts > 0); }
        }
    }
}