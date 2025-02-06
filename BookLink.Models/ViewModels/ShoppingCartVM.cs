using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.Models.ViewModels
{
	public class ShoppingCartVM
	{
		public IEnumerable<ShoppingCart> ListCart { get; set; }
		public OrderHeader OrderHeader { get; set; }
		public IEnumerable<SelectListItem> StateList { get; set; }

	}
}
