using System.Web;
using ChallengeBoard.Models;

namespace ChallengeBoard.Infrastucture
{
    public class SessionManager
    {
        public CompetitorDiscussion DiscussionMeta { get; set; }

        // private constructor.  No instantiation.
        private SessionManager() {}

        // Gets the current session.
        public static SessionManager Current
        {
            get
            {
                var session = (SessionManager)HttpContext.Current.Session["__Session__"];
                if (session == null)
                {
                    session = new SessionManager();
                    HttpContext.Current.Session["__Session__"] = session;
                }
                return session;
            }
        }
    }
}