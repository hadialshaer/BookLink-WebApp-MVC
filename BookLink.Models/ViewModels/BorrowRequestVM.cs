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
		public Book Book { get; set; }
		public User Lender { get; set; }
		public User Borrower { get; set; }
	}
}
