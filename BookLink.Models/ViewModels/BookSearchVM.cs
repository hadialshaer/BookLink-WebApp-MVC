using BookLink.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.Models.ViewModels
{
	public class BookSearchVM
	{
		public PaginatedList<Book> Books { get; set; }
		public string SearchString { get; set; }
		public string Category { get; set; }
		public TransactionType? TransactionType { get; set; }
		public AvailabilityStatus? Availability { get; set; }

		// Selection lists
		public IEnumerable<SelectListItem> Categories { get; set; }
		public IEnumerable<SelectListItem> TransactionTypes { get; set; }
		public IEnumerable<SelectListItem> AvailabilityOptions { get; set; }

		public List<int> WishlistBookIds { get; set; } = new List<int>();
	}
}
