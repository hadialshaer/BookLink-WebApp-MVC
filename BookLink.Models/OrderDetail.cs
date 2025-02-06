using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.Models
{
	public class OrderDetail
	{
		public int Id { get; set; }
		public int OrderHeaderId { get; set; }

		public int BookId { get; set; }
		public int Count { get; set; }
		public double Price { get; set; }
		public string Title { get; set; }
		


		[ForeignKey("BookId")]
		[ValidateNever]
		public Book Book { get; set; }

		[ForeignKey("OrderHeaderId")]
		[ValidateNever]
		public OrderHeader OrderHeader { get; set; }


	}
}
