using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CSharp.Mvc.Models;
using System.ComponentModel.Composition;

namespace CSharp.Mvc
{
	public class Global : HttpApplication
	{
		private static readonly TodoList s_todoList = new TodoList();

		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.MapRoute("Item", "item/{id}",
				new { controller = "TodoItem", action = "Index" });
			routes.MapRoute("Default", "{action}",
				new { controller = "Todo", action = "Index" });
		}

		[Export]
		public TodoList TodoList
		{
			get { return s_todoList; }
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}
	}
}
