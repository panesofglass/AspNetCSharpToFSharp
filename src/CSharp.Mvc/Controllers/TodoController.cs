using System.Web.Mvc;
using CSharp.Mvc.Models;

namespace CSharp.Mvc.Controllers {
	public class TodoController : Controller {
		private static readonly TodoList TodoList = new TodoList();

		public ActionResult Index() {
			return View(TodoList);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public ActionResult Index(TodoItem item) {
			if (ModelState.IsValid) {
				TodoList.Add(item);
				Response.StatusCode = (int) System.Net.HttpStatusCode.Created;
				return RedirectToAction("Index");
			}
			return View();
		}

		public ActionResult Create() {
			return View(new TodoItem());
		}
	}
}
