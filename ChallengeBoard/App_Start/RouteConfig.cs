using System.Web.Mvc;
using System.Web.Routing;

namespace ChallengeBoard
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "BoardMatchesRoute",
            //    url: "Matches/{id}",
            //    defaults: new { controller = "Matches", action = "Index", id = 0 }
            //    //defaults: new { controller = "Boards", action = "Index", id = 0 }
            //);

            //routes.MapRoute(
            //    name: "MatchDetailsRoute",
            //    url: "Matches/{action}/{boardId}/{matchId}",
            //    defaults: new { controller = "Matches", action = "Verify", boardId = 0, matchId = 0 }
            //);

            routes.LowercaseUrls = true;

            routes.MapRoute(
                name: "MatchesRoute",
                url: "Boards/Matches/{boardId}/{action}/{matchId}",
                defaults: new { controller = "Matches", action = "List", boardId = 0, matchId = 0 }
            );

            routes.MapRoute(
                name: "CompetitorRoute",
                url: "Boards/Competitors/{boardId}/{action}/{competitorId}",
                defaults: new { controller = "Competitors", action = "Profile", boardId = 0, competitorId = 0 }
            );

            routes.MapRoute(
                name: "DiscussionRoute",
                url: "Boards/Discussion/{boardId}/{action}/{postId}",
                defaults: new { controller = "Discussion", action = "Index", boardId = 0, postId = 0 }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}