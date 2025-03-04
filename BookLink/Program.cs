using BookLink.DataAccess.Data;
using BookLink.DataAccess.Repository;
using BookLink.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using BookLink.Utility;
using BookLink.Models;
using Stripe;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using BookLink.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

#region Configure Services (Dependency Injection)


builder.Services.AddMemoryCache();

// Add Razor Pages and MVC
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Configure Stripe Settings
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// Configure Database Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();

// Add Authentication Services (Google & Facebook)
builder.Services.AddAuthentication()
	.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
	{
		options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
		options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
	})
	.AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
	{
		options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
		options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
	});

// Configure Identity Cookie Options
builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Identity/Account/Login";
	options.LogoutPath = "/Identity/Account/Logout";
	options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Configure Session Management
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(100);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

// Register Repositories for Dependency Injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();


// Register DbInitializer for Dependency Injection
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

#endregion

var app = builder.Build();

#region Configure Middleware (Request Processing Pipeline)

// Error Handling
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

// Security & Static Files
app.UseHttpsRedirection();
app.UseStaticFiles();

// Configure Stripe API Key
StripeConfiguration.ApiKey = app.Configuration.GetSection("Stripe:SecretKey").Get<string>();

//  Enable Routing
app.UseRouting();

//  Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

//  Enable Session Middleware
app.UseSession();

//  Seed Database
SeedDataBase();

//  Map Razor Pages
app.MapRazorPages();

//  Configure Default Route
app.MapControllerRoute(
	name: "default",
	pattern: "{area=Member}/{controller=Home}/{action=Index}/{id?}");

#endregion

app.Run();

void SeedDataBase()
{
	using (var scope = app.Services.CreateScope())
	{
		var services = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
		services.Initialize();
	};
}
