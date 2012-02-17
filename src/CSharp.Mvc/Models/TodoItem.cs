using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CSharp.Mvc.Models
{
	public class TodoItem
	{
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

		public void MarkComplete()
		{
			Completed = DateTime.UtcNow;
		}

		public void Reset()
		{
			Completed = null;
		}
	}
}
