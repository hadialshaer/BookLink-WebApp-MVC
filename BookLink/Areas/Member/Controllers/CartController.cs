using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using BookLink.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookLink.Areas.Member.Controllers
{
	public class CartController : Controller
	{

		private readonly IUnitOfWork _unitOfWork;
		public ShoppingCartVM ShoppingCartVM { get; set; }

		public CartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		// Get Shopping Cart items
		[Area("Member")]
		[Authorize]
		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new ShoppingCartVM()
			{
				ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == userId,
				includeProperties: "Book"),
			};

			foreach(var cart in ShoppingCartVM.ListCart)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderTotal += (cart.Price * cart.Count);
			}

			return View(ShoppingCartVM);
		}

		private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
		{
			if (shoppingCart.Count <= 3)
			{
				return (double)shoppingCart.Book.Price;
			}
			else
			{
				if (shoppingCart.Count <= 5 )
				{
					return (double)shoppingCart.Book.Price3;
				}
				else
				{
					return (double)shoppingCart.Book.Price5;
				}
			}
		}
	}
}
