using System.Linq;
using System.Net;
using System.Web.Mvc;
using CSharp.Mvc.Models;

namespace CSharp.Mvc.Controllers
{
	public class TodoItemController : Controller
	{
		private readonly TodoList _todoList;

		public TodoItemController(TodoList todoList)
		{
			_todoList = todoList;
		}

		[HttpPut, ValidateAntiForgeryToken]
		public ActionResult Index(int id, TodoItem item)
		{
			var removed = _todoList.Items.SingleOrDefault(i => i.Id == id);
			if (removed == null)
				return new HttpNotFoundResult();

			if (!ModelState.IsValid)
				return new HttpStatusCodeResult((int) HttpStatusCode.BadRequest);

			_todoList.Items.Remove(removed);
			_todoList.Add(item);
			return RedirectToAction("Index", "Todo");
		}

		[HttpDelete, ValidateAntiForgeryToken]
		public ActionResult Index(int id)
		{
			var removed = _todoList.Items.SingleOrDefault(i => i.Id == id);
			if (removed == null)
				return new HttpNotFoundResult();

			_todoList.Items.Remove(removed);
			return RedirectToAction("Index", "Todo");
		}
	}
}
