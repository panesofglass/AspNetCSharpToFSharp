using System.Net;
using System.Web.Mvc;
using CSharp.Mvc.Models;

namespace CSharp.Mvc.Controllers
{
	public class TodoController : Controller
	{
		private readonly TodoList _todoList;

		public TodoController(TodoList todoList)
		{
			_todoList = todoList;
		}

		public ActionResult Index()
		{
			return View(_todoList);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public ActionResult Index(TodoItem item)
		{
			if (ModelState.IsValid)
			{
				_todoList.Add(item);
				Response.StatusCode = (int) HttpStatusCode.Created;
				return RedirectToAction("Index");
			}
			return View();
		}

		public ActionResult Create()
		{
			return View(new TodoItem());
		}
	}
}
