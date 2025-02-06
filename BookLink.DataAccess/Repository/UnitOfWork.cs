using BookLink.DataAccess.Data;
using BookLink.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.DataAccess.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private ApplicationDbContext _context;

		public ICategoryRepository Category { get; private set; }
		public IBookRepository Book { get; private set; }
		public IShoppingCartRepository ShoppingCart { get; private set; }
		public IUserRepository User { get; private set; }

		public IOrderDetailRepository OrderDetail { get; private set; }
		public IOrderHeaderRepository OrderHeader { get; private set; }

		public UnitOfWork(ApplicationDbContext context)
		{
			_context = context;
			Category = new CategoryRepository(_context);
			Book = new BookRepository(_context);
			ShoppingCart = new ShoppingCartRepository(_context);
			User = new UserRepository(_context);
			OrderDetail = new OrderDetailRepository(_context);
			OrderHeader = new OrderHeaderRepository(_context);
		}
		
		public void Save()
		{
			_context.SaveChanges();
		}
	}
}
