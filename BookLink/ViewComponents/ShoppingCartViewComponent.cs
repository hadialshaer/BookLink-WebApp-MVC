using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using BookLink.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookLink.ViewComponents
{
	public class ShoppingCartViewComponent : ViewComponent
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<User> _userManager;

		public ShoppingCartViewComponent(IUnitOfWork unitOfWork, UserManager<User> userManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var count = HttpContext.Session.GetInt32(SD.SessionCart);
			var userId = _userManager.GetUserId((System.Security.Claims.ClaimsPrincipal)User);

			if (userId != null)
			{
				if (count == null)
				{
					HttpContext.Session.SetInt32(SD.SessionCart,
					_unitOfWork.ShoppingCart.GetAll(u => u.UserId == userId).Count());
				}
				return View(count);

			}
			else
			{
				HttpContext.Session.Clear();
				return View(0);
			}

		}
	}
}
