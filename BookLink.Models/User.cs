using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.Models
{
	public class User : IdentityUser
	{
		public string PostalCode;

		[Required]
		[StringLength(50, ErrorMessage = "This name is not allowed", MinimumLength = 3)]
		public string FirstName { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "This last name is not allowed", MinimumLength = 4)]
		public string LastName { get; set; }

		public string? City { get; set; }

		[DataType(DataType.Date)]
		public DateTime BirthDate { get; set; }

		public Gender gender { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public string Address { get; set; }
	}

	public enum Gender{
		Male,
		Female
	}
}
