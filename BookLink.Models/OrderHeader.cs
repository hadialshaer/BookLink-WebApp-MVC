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
	public class OrderHeader
	{
		public int Id { get; set; }

		public string UserId { get; set; }


		[ForeignKey("UserId")]
		[ValidateNever]
		public User User { get; set; }

		public DateTime OrderDate { get; set; }

		public DateTime ShippingDate { get; set; }

		public double OrderTotal { get; set; }

		public string? OrderStatus { get; set; }

		public string? PaymentStatus { get; set; }

		public string? Carrier { get; set; }

		public DateTime PaymentDate { get; set; }

		public string? SessionId { get; set; }
		public string? PaymentIntentId { get; set; }





		// Billing Address
		[Required]
		public string PhoneNumber { get; set; }
		[Required]
		public string Address { get; set; }

		[Required]
		public string City { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string State { get; set; }

	}
}
