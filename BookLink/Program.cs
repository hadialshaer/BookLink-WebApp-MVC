using BookLink.DataAccess.Data;
using BookLink.DataAccess.Repository;
using BookLink.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using BookLink.Utility;
using BookLink.Models;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add Services to the DI Container
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Fetch Stripe Settings values from appsettings.json
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
// Configure Database Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();

// Configure Identity Options
builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = $"/Identity/Account/Login";
	options.LogoutPath = $"/Identity/Account/Logout";
	options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

// Add Session Support to Services
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
var app = builder.Build();

// Configure Middleware (Request Processing Pipeline)
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
StripeConfiguration.ApiKey = app.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
app.MapControllerRoute(
	name: "default",
	pattern: "{area=Member}/{controller=Home}/{action=Index}/{id?}");

app.Run();
