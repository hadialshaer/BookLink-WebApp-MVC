using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
	{
		ICategoryRepository Category { get; }

		IBookRepository Book { get; }

		IShoppingCartRepository ShoppingCart { get; }

		IUserRepository User { get; }

		IOrderDetailRepository OrderDetail { get; }

		IOrderHeaderRepository OrderHeader { get; }

		IBorrowRequestRepository BorrowRequest { get; }

		void Save();
	}
}
