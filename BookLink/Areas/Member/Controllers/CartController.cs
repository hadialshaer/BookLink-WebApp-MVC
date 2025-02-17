using BookLink.DataAccess.Repository.IRepository;
using BookLink.Models;
using BookLink.Models.ViewModels;
using BookLink.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe.Checkout;
using System.Security.Claims;

namespace BookLink.Areas.Member.Controllers
{
	[Area("Member")]
	[Authorize(Roles = SD.Role_Member)]
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
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked: true);

			if (cartFromDb.Count <= 1)
			{
				// Remove the item from the cart
				HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
					.GetAll(u => u.UserId == cartFromDb.UserId).Count() - 1);
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
			if (shoppingCart.Book == null || shoppingCart.Book.Price == null)
			{
				return 0; // Default price if null
			}

			if (shoppingCart.Count <= 3)
			{
				return (double)shoppingCart.Book.Price;
			}
			else
			{
				if (shoppingCart.Count <= 5)
				{
					return shoppingCart.Book.Price3 ?? shoppingCart.Book.Price.Value;
				}
				else
				{
					return shoppingCart.Book.Price5 ?? shoppingCart.Book.Price.Value;
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

			// Check if the cart is empty
			if (ShoppingCartVM.ListCart == null || !ShoppingCartVM.ListCart.Any())
			{
				TempData["Error"] = "Your cart is empty. Add items before proceeding to checkout.";
				return RedirectToAction("Index"); // Redirect back to the cart page
			}

			ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.UtcNow;
			ShoppingCartVM.OrderHeader.UserId = userId;

			User user = _unitOfWork.User.Get(u => u.Id == userId);

			foreach (var cart in ShoppingCartVM.ListCart)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			// Statuses
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
					Count = cart.Count,
					Title = cart.Book.Title
				};
				_unitOfWork.OrderDetail.Add(orderDetail);
				_unitOfWork.Save();
			}

			var domain = "https://localhost:7110/";

			// Option configuration
			var options = new SessionCreateOptions
			{
				SuccessUrl = domain + $"member/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
				CancelUrl = domain + "member/cart/index",
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment"
			};
			// End of option configuration

			foreach (var item in ShoppingCartVM.ListCart)
			{
				var sessionLineItem = new SessionLineItemOptions()
				{
					PriceData = new SessionLineItemPriceDataOptions()
					{
						UnitAmount = (long)item.Price * 100,
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions()
						{
							Name = item.Book.Title,
							Description = item.Book.Description
						}
					},
					Quantity = item.Count

				};
				options.LineItems.Add(sessionLineItem);
			}

			var service = new SessionService(); // create new session service
			Session session = service.Create(options);
			_unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.Save();


			Response.Headers.Append("Location", session.Url);
			return new StatusCodeResult(303);
		}



		[HttpDelete]
		public IActionResult Remove(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked:true);
			if (cartFromDb == null)
			{
				return Json(new { success = false, message = "Cart item not found" });
			}

			HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
				.GetAll(u => u.UserId == cartFromDb.UserId).Count() - 1);

			// Remove the item from the cart
			_unitOfWork.ShoppingCart.Remove(cartFromDb);

			_unitOfWork.Save();

			return Json(new { success = true, message = "Cart item removed successfully" });
		}



		public IActionResult OrderConfirmation(int id)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "User");

			var service = new SessionService();
			var session = service.Get(orderHeader.SessionId);

			if (session.PaymentStatus.ToLower() == "paid")
			{
				_unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
				_unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
				_unitOfWork.Save();
			}
			HttpContext.Session.Clear();

			List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
				.GetAll(u => u.UserId == orderHeader.UserId).ToList();

			_unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
			_unitOfWork.Save();

			return View(id);
		}

	}
}
