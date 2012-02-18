using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CSharp.Mvc.Models
{
	public class TodoItem
	{
		private static int s_index = 0;
		private readonly int _id;

		public TodoItem()
		{
			_id = s_index++;
		}

		[HiddenInput(DisplayValue = false)]
		public int Id
		{
			get { return _id; }
		}

		[Required]
		public string Name { get; set; }

		public DateTime? Due { get; set; }

		public DateTime? Completed { get; set; }

		[HiddenInput(DisplayValue = false)]
		public TodoState State
		{
			get
			{
				return Completed.HasValue ? TodoState.Complete :
				    !Due.HasValue || Due.Value <= DateTime.UtcNow ? TodoState.OnTime :
				    TodoState.Overdue;
			}
		}
	}
}
