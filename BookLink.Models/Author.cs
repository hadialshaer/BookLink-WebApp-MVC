using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.Models
{
	public class Author
	{
		[BindNever]
		public int AuthorId { get; set; }

		[Required]
		[StringLength(50, ErrorMessage = "this name is not allowed", MinimumLength = 2)]
		public string FirstName { get; set; }

		[Required]
		[StringLength(50, ErrorMessage = "this last name is not allowed", MinimumLength = 2)]
		public string LastName { get; set; }

		public virtual IEnumerable<Book> Books { get; set; }

	}
}
