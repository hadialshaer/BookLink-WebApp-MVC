﻿using Microsoft.AspNetCore.Mvc;
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

	// Lending and Borrowing properties
	[Required]
	[Display(Name = "Max Lend Duration (Days)")]
	[Range(1, 60, ErrorMessage = "Duration must be between 1 and 60 days")]
	public int MaxLendDurationDays { get; set; } = 14;

	[Required(ErrorMessage = "Setting a symbolic price for the book gives it tangible value.")]
	[Range(50000, 200000, ErrorMessage = "Borrowing fee must be between 50,000 and 200,000 ل.ل")]
	[Display(Name = "Borrowing Fee")]
	public int BorrowingFee { get; set; } = 100;

	[Range(1, int.MaxValue, ErrorMessage = "Number of pages must be at least 1")]
	public int NumberOfPages { get; set; }

	public DateTime? DueDate { get; set; }
	public string? BorrowerId { get; set; }

	[ForeignKey("BorrowerId")]
	[ValidateNever]
	public User? Borrower { get; set; }

	public BookStatus BookStatus { get; set; } = Models.BookStatus.Available;


	public string? LenderId { get; set; }

	[ForeignKey("LenderId")]
	[ValidateNever]
	public User? Lender { get; set; }

	public bool IsAvailable => BookStatus == BookStatus.Available;

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
public enum AvailabilityStatus
{
	Available,
	ComingSoon
}


