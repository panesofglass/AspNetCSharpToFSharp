using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CSharp.Mvc {
	public class MvcApplication : HttpApplication {
		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.MapRoute(
				"Default",
				"{action}/{id}",
				new { controller = "Todo", action = "Index", id = UrlParameter.Optional } );
		}

		protected void Application_Start() {
			AreaRegistration.RegisterAllAreas();
			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}
	}
}
