using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLink.Models;

public class Category
{

	[BindNever]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increments CategoryId
	public int CategoryId { get; set; }

	// Server side validation 1: Required attribute
	[Required(ErrorMessage = "Category Name is required")]
	[Display(Name = "Category Name")]
	public string CategoryName { get; set; }
}
