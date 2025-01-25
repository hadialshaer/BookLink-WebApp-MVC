using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLink.Models;

public class Book
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int BookId { get; set; }

	[Required(ErrorMessage = "The Title field is required.")]
	[StringLength(100, ErrorMessage ="Title must be 5-100 charachters", MinimumLength = 5) ]
	public string Title { get; set; }


	[Required]
	[StringLength(500, ErrorMessage = "Description must be 5-500 characters.", MinimumLength = 5)]
	public string Description { get; set; }

	[Required]
	public string Author { get; set; }

	[Required]
	[Display(Name ="List Price")]
	[Range(1,100, ErrorMessage = "Price must be between $1 and $100")]
	public double ListPrice { get; set; }

	[Required]
	[Display(Name = "Price for 1-3")]
	[Range(1, 1000, ErrorMessage = "Price must be between $1 and $1000")]
	public double Price { get; set; }

	[Required]
	[Display(Name = "Price for 3+")]
	[Range(1, 100, ErrorMessage = "Price must be between $1 and $100")]
	public double Price3 { get; set; }

	[Required]
	[Display(Name = "Price for 5+")]
	[Range(1, 100, ErrorMessage = "Price must be between $1 and $100")]
	public double Price5 { get; set; }

	

	


	//[Required]
	//public string CoverImageUrl { get; set; }

	//public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	//public DateTime? UpdatedAt { get; set; }

	//[BindNever]
	//[Display(Name = "Available")]
	//public bool IsAvailable { get; set; } = true; // Default to available


	/// ///////////////////////////////////////////////////////////////////////////////
	// Borrowing Details
	//public DateTime? BorrowedDate { get; set; } // When it was borrowed
	//public DateTime? DueDate { get; set; } // When it should be returned
	//////////////////////////////////////////////////////////////////////////////
	//[Required]
	//public bool IsForBorrow { get; set; } // True if available for borrowing

	//[Required]
	//public bool IsForSale { get; set; } // True if available for sale	

	//public string Condition { get; set; } // New, Good, Used, Damaged

}

