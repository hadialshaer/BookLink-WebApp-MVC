using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace BookLink.Models
{
	public class Book
	{
		[BindNever]
		public int BookId { get; set; }

		[Required(ErrorMessage = "The Title field is required.")]
		[StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.", MinimumLength = 5)]
		public string Title { get; set; }


		[Required]
		[StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.", MinimumLength = 5)]
		public string Description { get; set; }

		[Required]
		public string CoverImageUrl { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; }

		[BindNever]
		[Display(Name = "Available")]
		public bool IsAvailable { get; set; } = true; // Default to available

	}
}
