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
			// manual mapping
			var bookFromDb = _context.Books.FirstOrDefault(b => b.BookId == book.BookId);
			if (bookFromDb != null)
			{
				bookFromDb.Title = book.Title;
				bookFromDb.Author = book.Author;
				bookFromDb.Description = book.Description;
				bookFromDb.ListPrice = book.ListPrice;
				bookFromDb.Price = book.Price;
				bookFromDb.Price3 = book.Price3;
				bookFromDb.Price5 = book.Price5;			
				bookFromDb.CategoryId = book.CategoryId;

				if (book.ImageUrl != null)
				{
					bookFromDb.ImageUrl = book.ImageUrl;
				}

			}
		}
	}
}
