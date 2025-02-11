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

		public IEnumerable<SelectListItem> Loactions { get; set; }
	}
}
