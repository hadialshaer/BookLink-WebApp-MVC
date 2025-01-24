﻿using BookLink.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BookLink.DataAccess.Data;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{

	}

	public DbSet<Category> Categories { get; set; }
	public DbSet<Book> Books { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Category>().HasData(
			new Category { CategoryId = -1, CategoryName = "Literature" },
			new Category { CategoryId = -2, CategoryName = "History" },
			new Category { CategoryId = -3, CategoryName = "Economics" },
			new Category { CategoryId = -4, CategoryName = "Medicine & Health" },
			new Category { CategoryId = -5, CategoryName = "Religions & Beliefs" },
			new Category { CategoryId = -6, CategoryName = "Self-Development" },
			new Category { CategoryId = -7, CategoryName = "Science Fiction & Fantasy" },
			new Category { CategoryId = -8, CategoryName = "Philosophy" }
			);
		modelBuilder.Entity<Book>().HasData(
			new Book
			{
				BookId = -1,
				Title = "The Great Gatsby",
				Description = "The Great Gatsby is a 1925 novel by American writer F. Scott Fitzgerald. Set in the Jazz Age on Long Island, near New York City, the novel depicts first-person narrator Nick Carraway's interactions with mysterious millionaire Jay Gatsby and Gatsby's obsession to reunite with his former lover, Daisy Buchanan.",
				Author = "F. Scott Fitzgerald",
				ListPrice = 10.99,
				Price = 8.99,
				Price3 = 7.99,
				Price5 = 6.99
			},
			new Book
			{
				BookId = -2,
				Title = "The Catcher in the Rye",
				Description = "The Catcher in the Rye is a novel by J. D. Salinger, partially published in serial form in 1945–1946 and as a novel in 1951. It was originally intended for adults but is often read by adolescents for its themes of angst, alienation, and as a critique on superficiality in society.",
				Author = "J. D. Salinger",
				ListPrice = 12.99,
				Price = 10.99,
				Price3 = 9.99,
				Price5 = 8.99
			},
			new Book
			{
				BookId = -3,
				Title = "To Kill a Mockingbird",
				Description = "To Kill a Mockingbird is a novel by Harper Lee published in 1960. Instantly successful, widely read in high schools and middle schools in the United States, it has become a classic of modern American literature, winning the Pulitzer Prize.",
				Author = "Harper Lee",
				ListPrice = 14.99,
				Price = 12.99,
				Price3 = 11.99,
				Price5 = 10.99
			},
			new Book
			{
				BookId = -4,
				Title = "1984",
				Description = "1984 is a dystopian social science fiction novel by English novelist George Orwell. It was published on 8 June 1949 by Secker & Warburg as Orwell's ninth and final book completed in his lifetime.",
				Author = "George Orwell",
				ListPrice = 16.99,
				Price = 14.99,
				Price3 = 13.99,
				Price5 = 12.99
			},
			new Book
			{
				BookId = -5,
				Title = "Brave New World",
				Description = "Brave New World is a dystopian social science fiction novel by English author Aldous Huxley, written in 1931 and published in 1932. Largely set in a futuristic World State, whose citizens are environmentally engineered into an intelligence-based social hierarchy.",
				Author = "Aldous Huxley",
				ListPrice = 18.99,
				Price = 16.99,
				Price3 = 15.99,
				Price5 = 14.99
			},
			new Book
			{
				BookId = -6,
				Title = "The Lord of the Rings",
				Description = "The Lord of the Rings is an epic high-fantasy novel by English author and scholar J. R. R. Tolkien. Set in Middle-earth, the world at some distant time in the past, the story began as a sequel to Tolkien's 1937 children's book The Hobbit, but eventually developed into a much larger work.",
				Author = "J. R. R. Tolkien",
				ListPrice = 20.99,
				Price = 18.99,
				Price3 = 17.99,
				Price5 = 16.99
			}

			);
		
	}

}
