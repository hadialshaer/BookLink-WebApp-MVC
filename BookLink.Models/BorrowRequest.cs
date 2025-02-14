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
	public class BorrowRequest
	{
		public int Id { get; set; }

		public int BookId { get; set; }

		public string BorrowerId { get; set; }
		public string LenderId { get; set; }
		public DateTime RequestDate { get; set; } = DateTime.UtcNow;
		public DateTime? ApprovalDate { get; set; }
		public DateTime? DueDate { get; set; }
		public DateTime? ReturnDate { get; set; }
		public BorrowRequestStatus Status { get; set; } = BorrowRequestStatus.Pending;

		[ForeignKey("BookId")]
		[ValidateNever]
		public Book Book { get; set; }

		[ForeignKey("LenderId")]
		[ValidateNever]
		public User Lender { get; set; }

		[ForeignKey("BorrowerId")]
		[ValidateNever]
		public User Borrower { get; set; }

		// Borrowing properties

		[Required(ErrorMessage = "Please select a location")]
		public int LocationId { get; set; }

		[Required(ErrorMessage = "Please enter Borrower Phone Number")]
		[Phone(ErrorMessage = "Please enter a Valid Phone Number")]
		public string Phone { get; set; }

		[EmailAddress(ErrorMessage = "Please enter a Email Address")]
		public string BorrowerEmail { get; set; }

		[Required(ErrorMessage = "Please enter your Name")]
		public string BorrowerName { get; set; } 

		[ForeignKey("LocationId")]
		[ValidateNever]
		public Location Location { get; set; }  

	}

	public enum BorrowRequestStatus
	{
		Pending,
		Approved,
		Rejected,
		Returned,
		Overdue
	}
}
