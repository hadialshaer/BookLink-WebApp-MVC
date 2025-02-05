using BookLink.DataAccess.Data;
using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.DataAccess.Repository
{
	public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
	{
		private ApplicationDbContext _context;
		public ShoppingCartRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(ShoppingCart shoppingCart)
		{
			_context.shoppingCarts.Update(shoppingCart);
		}
	}
}
