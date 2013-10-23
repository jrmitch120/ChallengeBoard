using System.Web;
using System.Web.Mvc;
using ChallengeBoard.Services;

namespace ChallengeBoard.Infrastucture
{
    public class MetaRequestAttribute : ActionFilterAttribute
    {
        private readonly IBoardService _boardService;
        private readonly string _boardIdParameter;

        public MetaRequestAttribute(string boardIdParameter)
        {
            // I can't figure out how to do property injection on an Attribute using Munq.
            // I have to resort to this... =(
            _boardService = DependencyResolver.Current.GetService<IBoardService>();
            _boardIdParameter = boardIdParameter;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated || !filterContext.ActionParameters.ContainsKey(_boardIdParameter))
                SessionManager.Current.DiscussionMeta = null;
            else
            {
                var id = (int)filterContext.ActionParameters[_boardIdParameter];
                SessionManager.Current.DiscussionMeta = _boardService.GetDiscussionMeta(id, HttpContext.Current.User.Identity.Name);
            }
        }
    }
}