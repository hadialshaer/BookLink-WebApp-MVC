using BookLink.DataAccess.Data;
using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using Microsoft.EntityFrameworkCore;
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

				// Update image only if provided
				if (book.ImageUrl != null)
				{
					bookFromDb.ImageUrl = book.ImageUrl;
				}

				// Additional properties related to lending/borrowing
				bookFromDb.TransactionType = book.TransactionType;
				bookFromDb.MaxLendDurationDays = book.MaxLendDurationDays;
				bookFromDb.DueDate = book.DueDate;
				bookFromDb.BorrowerId = book.BorrowerId;
				bookFromDb.LenderId = book.LenderId;
				bookFromDb.BookStatus = book.BookStatus;
			}
		}

		public IQueryable<Book> FullTextSearch(string searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return _context.Books.AsQueryable();

			// Sanitize input
			var cleanTerm = searchTerm.Trim()
				.Replace("\"", "")  // Remove quotes
				.Replace("*", "")   // Remove wildcards
				.Replace("'", "''"); // Escape single quotes

			// Split and format terms
			var terms = cleanTerm.Split()
				.Where(t => t.Length > 0)
				.Select(t => $"\"{t}*\"")
				.ToList();

			if (!terms.Any()) return _context.Books.AsQueryable();

			var searchQuery = string.Join(" AND ", terms);

			return _context.Books
				.FromSqlRaw($"SELECT * FROM Books WHERE CONTAINS((Title, Author, Description), {{0}})", searchQuery)
				.AsNoTracking();
		}
	}
}
