using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace ChallengeBoard.Email
{
    public class SmtpPostmaster : IPostmaster
    {
        public void Send(ICollection<EmailContact> to, string subject, string body)
        {
            var message = new MailMessage { From = new MailAddress("test@gmail.com", "The Challenge Board") }; //TODO Web.config
            foreach (var contact in to)
                message.To.Add(new MailAddress(contact.EmailAddress, contact.Name));

            message.Subject = subject;
            message.Body = body;

            var client = new SmtpClient();
            client.Send(message);
        }
    }
}