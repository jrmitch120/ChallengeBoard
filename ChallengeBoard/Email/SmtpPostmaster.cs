using System.Collections.Generic;
using System.Net.Mail;

namespace ChallengeBoard.Email
{
    public class SmtpPostmaster : IPostmaster
    {
        private readonly string _fromEmail;
        private readonly string _fromName;

        public SmtpPostmaster(){}
        public SmtpPostmaster(string fromEmail, string fromName)
        {
            _fromEmail = fromEmail;
            _fromName = fromName;
        }

        public void Send(EmailContact to, string subject, string body)
        {
            Send(new[] {to}, subject, body);
        }
        public void Send(ICollection<EmailContact> to, string subject, string body)
        {
            var message = new MailMessage();

            if (_fromEmail != null)
                message.From = new MailAddress(_fromEmail, _fromName);

            foreach (var contact in to)
                message.To.Add(new MailAddress(contact.EmailAddress, contact.Name));
            
            message.IsBodyHtml = true;

            message.Subject = subject;
            message.Body = body;

            var client = new SmtpClient();
            client.Send(message);
        }
    }
}