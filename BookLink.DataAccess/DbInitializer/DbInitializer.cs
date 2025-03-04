using BookLink.DataAccess.Data;
using BookLink.Models;
using BookLink.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLink.DataAccess.DbInitializer
{
	public class DbInitializer : IDbInitializer
	{
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext _context;

		public DbInitializer(
			UserManager<User> userManager,
			RoleManager<IdentityRole> roleManager,
			ApplicationDbContext context)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_context = context;
		}

		public void Initialize()
		{
			// push migration if not applied
			try
			{
				if (_context.Database.GetPendingMigrations().Count() > 0)
				{
					_context.Database.Migrate();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}


			// create roles if not created

			if (!_roleManager.RoleExistsAsync(SD.Role_Member).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Member)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Guest)).GetAwaiter().GetResult();

				// create admin user, if roles not created
				_userManager.CreateAsync(new User
				{
					UserName = "admin@BookLink.com",
					Email = "admin@BookLink.com",
					FirstName = "Hadi",
					LastName = "Alshaer",
					State = "South",
					City = "Tyre",
					gender = Gender.Male,
					BirthDate = DateTime.UtcNow,
					Address = "N/A"
				},
				"!A12341234a").GetAwaiter().GetResult();

				User user = _context.users.FirstOrDefault(u => u.Email == "admin@BookLink.com");
				_userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
			}

			return;
		}
	}
}
