using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using BookLink.Models.ViewModels;
using BookLink.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BookLink.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
	{

		private readonly IUnitOfWork _unitOfWork;
		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Details(int orderId)
		{
			OrderVM orderVM = new()
			{
				OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "User"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(o => o.OrderHeaderId == orderId, includeProperties: "Book")
			};
			return View(orderVM);
		}

		#region API CALLS

		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "User").ToList();

			switch (status)
			{
				case "pending":
					objOrderHeaders = objOrderHeaders.Where(o => o.PaymentStatus == SD.PaymentStatusPending);
					break;
				case "inprocess":
					objOrderHeaders = objOrderHeaders.Where(o => o.OrderStatus == SD.StatusInProcess);
					break;
				case "completed":
					objOrderHeaders = objOrderHeaders.Where(o => o.OrderStatus == SD.StatusShipped);
					break;
				case "approved":
					objOrderHeaders = objOrderHeaders.Where(o => o.OrderStatus == SD.StatusApproved);
					break;
				default: 
					break;
			}

			return Json(new { data = objOrderHeaders });
		}
	}
	#endregion
}
