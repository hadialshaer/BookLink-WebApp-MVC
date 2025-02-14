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
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private ApplicationDbContext _context;
		public OrderHeaderRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(OrderHeader orderHeader)
		{
			_context.orderHeaders.Update(orderHeader);
		}

		public void UpdateStatus(int id, string status, string? paymentStatus = null)
		{
			var orderFromDb = _context.orderHeaders.FirstOrDefault(o => o.Id == id);
			if (orderFromDb != null)
			{
				orderFromDb.OrderStatus = status;
				if (!string.IsNullOrEmpty(paymentStatus))
				{
					orderFromDb.PaymentStatus = paymentStatus;
				}
			}
		}

		public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
		{
			var orderFromDb = _context.orderHeaders.FirstOrDefault(o => o.Id == id);
			if (!string.IsNullOrEmpty(sessionId))
			{
				orderFromDb.SessionId = sessionId;

			}
			if (!string.IsNullOrEmpty(paymentIntentId))
			{
				orderFromDb.PaymentIntentId = paymentIntentId;
				orderFromDb.PaymentDate = DateTime.UtcNow;

			}

		}
	}
}
