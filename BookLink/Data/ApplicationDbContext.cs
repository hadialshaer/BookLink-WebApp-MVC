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
				new Category { CategoryName = "Literature" },
				new Category { CategoryName = "History" },
				new Category { CategoryName = "Economics" },
				new Category { CategoryName = "Medicine & Health" },
				new Category { CategoryName = "Religions & Beliefs" },
				new Category { CategoryName = "Self-Development" },
				new Category { CategoryName = "Science Fiction & Fantasy" },
				new Category { CategoryName = "Philosophy" }
				);
		}

	}
}
