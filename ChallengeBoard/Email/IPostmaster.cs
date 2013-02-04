using System.Collections.Generic;

namespace ChallengeBoard.Email
{
    public interface IPostmaster
    {
        void Send(ICollection<EmailContact> to, string subject, string body);
    }
}