using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace BookLink.Models
{
	public class Category
	{

		[BindNever]
		public int CategoryId { get; set; }

		// Server side validation 1: Required attribute
		[Required(ErrorMessage = "Category Name is required")]
		[Display(Name = "Category Name")]
		public string CategoryName { get; set; }
	}
}