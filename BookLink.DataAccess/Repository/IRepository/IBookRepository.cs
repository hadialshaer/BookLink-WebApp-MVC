using BookLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.DataAccess.Repository.IRepository
{
	public interface IBookRepository : IRepository<Book> // Book is a model
	{
		void Update(Book book);

	}
}
