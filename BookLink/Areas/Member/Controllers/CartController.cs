using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using BookLink.Models.ViewModels;
using BookLink.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace BookLink.Areas.Member.Controllers
{
	[Area("Member")]
	public class CartController : Controller
	{

		private readonly IUnitOfWork _unitOfWork;

		[BindProperty]
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
				OrderHeader = new()
			};

			foreach (var cart in ShoppingCartVM.ListCart)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			return View(ShoppingCartVM);
		}

		public IActionResult CheckOut()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new ShoppingCartVM()
			{
				ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == userId,
				includeProperties: "Book"),
				OrderHeader = new()
			};
			ShoppingCartVM.OrderHeader.User = _unitOfWork.User.Get(u => u.Id == userId);


			string name = ShoppingCartVM.OrderHeader.User.FirstName + " " + ShoppingCartVM.OrderHeader.User.LastName;
			ShoppingCartVM.OrderHeader.Name = name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.User.PhoneNumber;
			ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.User.Address;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.User.City;
			ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.User.PostalCode;

			ShoppingCartVM.StateList = new List<SelectListItem>
			{
				new SelectListItem { Value = "", Text = "Choose..." },
				new SelectListItem { Value = "Beirut", Text = "Beirut" },
				new SelectListItem { Value = "South", Text = "South" },
				new SelectListItem { Value = "North", Text = "North" },
				new SelectListItem { Value = "Baalbek-Hermel", Text = "Baalbek-Hermel" }
			};


			foreach (var cart in ShoppingCartVM.ListCart)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
			return View(ShoppingCartVM);
		}

		public IActionResult Plus(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
			cartFromDb.Count += 1;
			_unitOfWork.ShoppingCart.Update(cartFromDb);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Minus(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);

			if (cartFromDb.Count <= 1)
			{
				// Remove the item from the cart
				_unitOfWork.ShoppingCart.Remove(cartFromDb);
			}
			else
			{
				cartFromDb.Count -= 1;
				_unitOfWork.ShoppingCart.Update(cartFromDb);
			}
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		

		private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
		{
			if (shoppingCart.Count <= 3)
			{
				return (double)shoppingCart.Book.Price;
			}
			else
			{
				if (shoppingCart.Count <= 5)
				{
					return (double)shoppingCart.Book.Price3;
				}
				else
				{
					return (double)shoppingCart.Book.Price5;
				}
			}
		}

		[HttpPost]
		[ActionName("CheckOut")]
		public IActionResult CheckOutPost()
		{
			// Order Header
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == userId,
				includeProperties: "Book");

			ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
			ShoppingCartVM.OrderHeader.UserId = userId;


			foreach (var cart in ShoppingCartVM.ListCart)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
			ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;

			_unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
			_unitOfWork.Save();

			// Order Detail
			foreach (var cart in ShoppingCartVM.ListCart)
			{
				OrderDetail orderDetail = new()
				{
					BookId = cart.BookId,
					OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
					Price = cart.Price,
					Count = cart.Count
				};
				_unitOfWork.OrderDetail.Add(orderDetail);
				_unitOfWork.Save();
			}

				return View(ShoppingCartVM);
		}

		[HttpDelete]
		public IActionResult Remove(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
			if (cartFromDb == null)
			{
				return Json(new { success = false, message = "Cart item not found" });
			}

			_unitOfWork.ShoppingCart.Remove(cartFromDb);
			_unitOfWork.Save();

			return Json(new { success = true, message = "Cart item removed successfully" });
		}


	}
}
