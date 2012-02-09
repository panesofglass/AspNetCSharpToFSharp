using System.Web.Mvc;

namespace CSharp.Mvc.Controllers
{
	public class HomeController : Controller
	{
		//
		// GET: /Home/

		public ActionResult Index()
		{
			return View();
		}

	}
}
