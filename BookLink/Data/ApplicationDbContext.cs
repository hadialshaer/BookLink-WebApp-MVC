using BookLink.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLink.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		public DbSet<Book> Books { get; set; }
		public DbSet<Category> Categories { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Category>().HasData(
				new Category { CategoryId = 1, CategoryName = "Literature" },
				new Category { CategoryId = 2, CategoryName = "History" },
				new Category { CategoryId = 3, CategoryName = "Economics" },
				new Category { CategoryId = 4, CategoryName = "Medicine & Health" },
				new Category { CategoryId = 5, CategoryName = "Religions & Beliefs" },
				new Category { CategoryId = 6, CategoryName = "Self-Development" },
				new Category { CategoryId = 7, CategoryName = "Science Fiction & Fantasy" },
				new Category { CategoryId = 8, CategoryName = "Philosophy" }
				);
		}

	}
}
