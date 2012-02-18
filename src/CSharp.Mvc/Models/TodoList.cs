using System.Collections.Generic;
using System.Linq;

namespace CSharp.Mvc.Models
{
	public class TodoList
	{
		private readonly IList<TodoItem> _todoItems;

		public TodoList()
		{
			_todoItems = new List<TodoItem>();
		}

		public IList<TodoItem> Items
		{
			get { return _todoItems; }
		}

		public TodoState State
		{
			get { return _todoItems.Any() ? _todoItems.Max(i => i.State) : TodoState.OnTime; }
		}

		public void Add(TodoItem item)
		{
			_todoItems.Add(item);
		}

		public void Remove(TodoItem item)
		{
			_todoItems.Remove(item);
		}
	}
}
