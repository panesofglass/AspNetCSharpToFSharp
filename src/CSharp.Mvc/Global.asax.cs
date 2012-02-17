using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CSharp.Mvc.Models;

namespace CSharp.Mvc
{
	public class Global : HttpApplication
	{
		public static readonly TodoList TodoList = new TodoList();

		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.MapRoute(
				"Default",
				"{action}/{id}",
				new { controller = "Todo", action = "Index", id = UrlParameter.Optional });
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}
	}
}
