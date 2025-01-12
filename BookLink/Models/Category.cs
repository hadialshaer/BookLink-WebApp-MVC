﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookLink.Models
{
	public class Category
	{
		[BindNever]
		public int CategoryId { get; set; }

		
		[Required(ErrorMessage = "Category Name is required")]
		[DisplayName("Category Name")]
		public string CategoryName { get; set; }
	}
}