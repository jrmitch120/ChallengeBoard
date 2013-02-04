using System;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Threading;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ChallengeBoard.Services;
using Munq.MVC3;

namespace ChallengeBoard
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static bool _sweeping;
// ReSharper disable NotAccessedField.Local
        private static Timer _timer;
// ReSharper restore NotAccessedField.Local
        private static readonly TimeSpan SweepInterval = TimeSpan.FromSeconds(30);

        protected void Application_Start()
        {
            //Database.SetInitializer(new DropCreateDatabaseAlways<ChallengeBoardContext>());
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ChallengeBoardContext>());

            DoMigrations();

            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            var ioc = MunqDependencyResolver.Container;
            
            var serviceFactory = new Func<IMatchService>(ioc.Resolve<IMatchService>);
            _timer = new Timer(_ => Sweep(serviceFactory), null, SweepInterval, SweepInterval);
        }

        private static void DoMigrations()
        {
            var settings = new Migrations.Configuration();
            var migrator = new DbMigrator(settings);
            migrator.Update();
        }

        private static void Sweep(Func<IMatchService> serviceFactory)
        {
            if (_sweeping || !Convert.ToBoolean(ConfigurationManager.AppSettings["matchSweeperEnabled"]))
            {
                return;
            }

            _sweeping = true;

            try
            {
                IMatchService srv = serviceFactory();
                srv.SweepMatches();
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            finally
            {
                _sweeping = false;
            }
        }
    }
}