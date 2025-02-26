using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.Models
{
	public class Wishlist
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public int BookId { get; set; }
		public DateTime AddedDate { get; set; } = DateTime.UtcNow;
		public WishlistStatus Status { get; set; } = WishlistStatus.ToRead;

		// Navigation properties
		[ForeignKey("UserId")]
		[ValidateNever]
		public User User { get; set; }
		[ForeignKey("BookId")]
		[ValidateNever]
		public Book Book { get; set; }
	}

	public enum WishlistStatus
	{
		ToRead,
		ToBorrow,
		ToPurchase
	}
}
