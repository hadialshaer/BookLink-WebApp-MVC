using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookLink.Models;

namespace BookLink.Areas.Identity.Pages.Account.Manage
{
	public class IndexModel : PageModel
	{
		// Inject UserManager and SignInManager for user management
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;

		public IndexModel(
			UserManager<User> userManager,
			SignInManager<User> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		// Status message to display success or error messages
		[TempData]
		public string StatusMessage { get; set; }

		// Bindable input model for form data
		[BindProperty]
		public InputModel Input { get; set; }

		// Username of the current user
		public string Username { get; set; }

		// Input model for profile data
		public class InputModel
		{
			[Required]
			[Display(Name = "First Name")]
			public string FirstName { get; set; }

			[Required]
			[Display(Name = "Last Name")]
			public string LastName { get; set; }

			[Phone]
			[Display(Name = "Phone Number")]
			public string PhoneNumber { get; set; }

			[Display(Name = "City")]
			public string City { get; set; }

			[DataType(DataType.Date)]
			[Display(Name = "Birth Date")]
			public DateTime? BirthDate { get; set; } // Nullable to allow optional input

			[Display(Name = "Gender")]
			public Gender gender { get; set; } // Enum for gender (e.g., Male = 0, Female = 1)
		}

		// Load user data into the Input model
		private async Task LoadAsync(User user)
		{
			var userName = await _userManager.GetUserNameAsync(user);
			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

			Username = userName ?? string.Empty;

			Input = new InputModel
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				PhoneNumber = phoneNumber,
				City = user.City,
				BirthDate = user.BirthDate,
			};
		}

		// Handle GET request to load the profile page
		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			await LoadAsync(user);
			return Page();
		}

		// Handle POST request to update the profile
		public async Task<IActionResult> OnPostAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			if (!ModelState.IsValid)
			{
				await LoadAsync(user);
				return Page();
			}

			// Update user properties
			user.FirstName = Input.FirstName;
			user.LastName = Input.LastName;
			user.PhoneNumber = Input.PhoneNumber;
			user.City = Input.City;

			// Save changes to the database
			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				StatusMessage = "Unexpected error when updating profile.";
				return RedirectToPage();
			}

			// Refresh sign-in to apply changes
			await _signInManager.RefreshSignInAsync(user);
			StatusMessage = "Your profile has been updated.";
			return RedirectToPage();
		}
	}
}