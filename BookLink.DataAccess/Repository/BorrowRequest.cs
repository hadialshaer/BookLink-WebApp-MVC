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
	public class BorrowRequestRepository : Repository<BorrowRequest>, IBorrowRequestRepository
	{
		private ApplicationDbContext _context;
		public BorrowRequestRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(BorrowRequest borrowRequest)
		{
			_context.BorrowRequests.Update(borrowRequest);
		}
	}
}
