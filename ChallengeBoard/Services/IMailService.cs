using ChallengeBoard.Email;
using ChallengeBoard.Email.Models;

namespace ChallengeBoard.Services
{
    public interface IMailService
    {
        void SendEmail<T>(string emailAddress, string emailName, string subject, EmailType emailType, T model)
            where T : IEmailModel;
    }
}