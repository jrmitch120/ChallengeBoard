using System.Web.Mvc;
using ChallengeBoard.Email;
using ChallengeBoard.Models;
using ChallengeBoard.Services;
using Munq.MVC3;

[assembly: WebActivator.PreApplicationStartMethod(
	typeof(ChallengeBoard.App_Start.MunqMvc4Startup), "PreStart")]

namespace ChallengeBoard.App_Start {
	public static class MunqMvc4Startup {
		public static void PreStart() {
			DependencyResolver.SetResolver(new MunqDependencyResolver());

			var ioc = MunqDependencyResolver.Container;

            //ioc.UsesDefaultLifetimeManagerOf(new Munq.LifetimeManagers.RequestLifetime());
            // Munq.Configuration.ConfigurationLoader.FindAndRegisterDependencies(ioc); // Optional
			// ioc.Register<IMyRepository, MyRepository>();
			// ...

		    ioc.Register<IRepository, PersistedRepository>();
            ioc.Register<IMatchService, MatchService>();
            ioc.Register<ChallengeBoardContext, ChallengeBoardContext>();
            ioc.Register<IPostmaster, SmtpPostmaster>();
		}
	}
}
 

