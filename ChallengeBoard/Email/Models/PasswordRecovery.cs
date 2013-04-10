namespace ChallengeBoard.Email.Models
{
    public class PasswordRecovery : IEmailModel
    {
        public string UserName { get; set; }
        public string RecoveryLink { get; set; }
    }
}