using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLink.Models;

public class Book
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int BookId { get; set; }

	[Required(ErrorMessage = "The Title field is required.")]
	[StringLength(100, ErrorMessage = "Title must be 5-100 charachters", MinimumLength = 5)]
	public string Title { get; set; }


	[Required]
	[StringLength(500, ErrorMessage = "Description must be 5-500 characters.", MinimumLength = 5)]
	public string Description { get; set; }

	[Required]
	public string Author { get; set; }


	[Display(Name = "List Price")]
	[Range(1, 100, ErrorMessage = "Price must be between $1 and $100")]
	public double? ListPrice { get; set; }

	[Display(Name = "Price for 1-3")]
	[Range(1, 1000, ErrorMessage = "Price must be between $1 and $1000")]
	public double? Price { get; set; }

	[Display(Name = "Price for 3+")]
	[Range(1, 100, ErrorMessage = "Price must be between $1 and $100")]
	public double? Price3 { get; set; }


	[Display(Name = "Price for 5+")]
	[Range(1, 100, ErrorMessage = "Price must be between $1 and $100")]
	public double? Price5 { get; set; }

	[Required]
	public int CategoryId { get; set; }

	// Navigations
	[ForeignKey("CategoryId")]
	[ValidateNever]
	public Category BookCategory { get; set; }

	[Required]
	[ValidateNever]
	public string ImageUrl { get; set; }


	[Required]
	public TransactionType TransactionType { get; set; }

	// Lending properties
	[Display(Name = "Max Lend Duration (Days)")]
	[Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
	public int? MaxLendDurationDays { get; set; }

	public DateTime? DueDate { get; set; }
	public string? BorrowerId { get; set; }

	[ForeignKey("BorrowerId")]
	[ValidateNever]
	public User? Borrower { get; set; }

	public BookStatus? BookStatus { get; set; } = Models.BookStatus.Available;

}
public enum TransactionType
{
	Sell,
	Lend
}
public enum BookStatus
{
	Available,   
	Borrowed,    
	Sold,        
	Pending      
}


