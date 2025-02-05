using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.Models
{
	public class ShoppingCart
	{
		public int Id { get; set; }

		[Range(1,100, ErrorMessage = "Please enter a value between 1-100")]
		public int Count { get; set; }


		public string UserId { get; set; }

		[ForeignKey("UserId")]
		[ValidateNever]
		public User User { get; set; }
		
		
		public int BookId { get; set; }

		[ForeignKey("BookId")]
		[ValidateNever]
		public Book Book { get; set; }
	}
}
