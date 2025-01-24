using BookLink.DataAccess.Data;
using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.DataAccess.Repository
{
	public class BookRepository : Repository<Book>, IBookRepository
	{
		private ApplicationDbContext _context;

		public BookRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(Book book)
		{
			_context.Books.Update(book);
		}
	}
}
