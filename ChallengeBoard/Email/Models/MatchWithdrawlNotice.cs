namespace ChallengeBoard.Email.Models
{
    public class MatchWithdrawlNotice : IEmailModel
    {
        public string Withdrawee { get; set; }
        public string Withdrawer { get; set; }
        public string BoardName { get; set; }
    }
}