﻿using BookLink.DataAccess.Data;
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
	public class WishlistRepository : Repository<Wishlist>, IWishlistRepository
	{
		private ApplicationDbContext _context;
		public WishlistRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(Wishlist wishList)
		{
			_context.Wishlists.Update(wishList);
		}
	}
}
