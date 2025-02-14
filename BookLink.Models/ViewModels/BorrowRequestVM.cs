using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.Models.ViewModels
{
	public class BorrowRequestVM
	{
		public BorrowRequest BorrowRequest { get; set; }

		public IEnumerable<SelectListItem> LocationList { get; set; } = new List<SelectListItem>();

		public bool IsBorrowed => BorrowRequest?.Book?.BookStatus == BookStatus.Borrowed;
	}
}
