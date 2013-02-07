namespace ChallengeBoard.Email.Models
{
    public class MatchNotification : IEmailModel
    {
        public string WinnerName { get; set; }
        public string LoserName { get; set; }
        public string BoardName { get; set; }
        public int AutoVerifies { get; set; }
    }
}