﻿using Microsoft.AspNetCore.Mvc;

namespace BookLink.Controllers
{
	public class BookController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}