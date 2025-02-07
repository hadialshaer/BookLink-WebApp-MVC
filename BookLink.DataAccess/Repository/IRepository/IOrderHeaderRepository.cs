using BookLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.DataAccess.Repository.IRepository
{
	public interface IOrderHeaderRepository : IRepository<OrderHeader>
	{
		void Update(OrderHeader orderHeader);

		void UpdateStatus(int id, string status, string? paymentStatus = null);

		void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);

	}
}
