using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace BookLink.Models
{
	public class Category
	{
		[BindNever]
		public int CategoryId { get; set; }

		[Required]
		public string CategoryName { get; set; }
	}
}
