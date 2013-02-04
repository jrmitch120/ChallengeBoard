using System.Collections.Generic;

namespace ChallengeBoard.Email
{
    public interface IPostmaster
    {
        void Send(EmailContact to, string subject, string body);
        void Send(ICollection<EmailContact> to, string subject, string body);
    }
}