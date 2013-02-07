using ChallengeBoard.Email;
using ChallengeBoard.Email.Models;

namespace ChallengeBoard.Services
{
    public class MailService : IMailService
    {
        private readonly IPostmaster _postmaster;

        public MailService(IPostmaster postmaster)
        {
            _postmaster = postmaster;
        }

        public void SendEmail<T>(string emailAddress, string emailName, string subject, EmailType emailType, T model) where T : IEmailModel
        {
            var message = EmailFactory.ParseTemplate(model, emailType);
            _postmaster.Send(new EmailContact(emailAddress, emailName), subject, message);
        }
    }
}