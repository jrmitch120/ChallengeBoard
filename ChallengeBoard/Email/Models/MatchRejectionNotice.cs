namespace ChallengeBoard.Email.Models
{
    public class MatchRejectionNotice : IEmailModel
    {
        public string RejectedName { get; set; }
        public string RejectorName { get; set; }
        public string BoardName { get; set; }
        public string BoardOwnerName { get; set; }
    }
}