using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.Utility
{
	public static class SD
	{
		// Roles
		public const string Role_Admin = "Admin";
		public const string Role_Member = "Member";
		public const string Role_Guest = "Guest";


		// Order Status
		public const string StatusPending = "Pending";
		public const string StatusApproved = "Approved";
		public const string StatusRejected = "Processing";
		public const string StatusShipped = "Shipped";
		public const string StatusCancelled = "Cancelled";
		public const string StatusRefunded = "Refunded";

		// Payment Status
		public const string PaymentStatusPending = "Pending";
		public const string PaymentStatusApproved = "Approved";
		public const string PaymentStatusRejected = "Rejected";

	}
}
