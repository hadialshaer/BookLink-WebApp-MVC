using BookLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.DataAccess.Repository.IRepository
{
	public interface IBorrowRequestRepository : IRepository<BorrowRequest>
	{
		void Update(BorrowRequest borrowRequest);
		
	}
}
