﻿// <auto-generated />
using BookLink.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BookLink.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BookLink.Models.Book", b =>
                {
                    b.Property<int>("BookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BookId"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ListPrice")
                        .HasColumnType("float");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<double>("Price3")
                        .HasColumnType("float");

                    b.Property<double>("Price5")
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("BookId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Books");

                    b.HasData(
                        new
                        {
                            BookId = 1,
                            Author = "F. Scott Fitzgerald",
                            CategoryId = 1,
                            Description = "The Great Gatsby is a 1925 novel by American writer F. Scott Fitzgerald. Set in the Jazz Age on Long Island, near New York City, the novel depicts first-person narrator Nick Carraway's interactions with mysterious millionaire Jay Gatsby and Gatsby's obsession to reunite with his former lover, Daisy Buchanan.",
                            ImageUrl = "",
                            ListPrice = 10.99,
                            Price = 8.9900000000000002,
                            Price3 = 7.9900000000000002,
                            Price5 = 6.9900000000000002,
                            Title = "The Great Gatsby"
                        },
                        new
                        {
                            BookId = 2,
                            Author = "J. D. Salinger",
                            CategoryId = 1,
                            Description = "The Catcher in the Rye is a novel by J. D. Salinger, partially published in serial form in 1945–1946 and as a novel in 1951. It was originally intended for adults but is often read by adolescents for its themes of angst, alienation, and as a critique on superficiality in society.",
                            ImageUrl = "",
                            ListPrice = 12.99,
                            Price = 10.99,
                            Price3 = 9.9900000000000002,
                            Price5 = 8.9900000000000002,
                            Title = "The Catcher in the Rye"
                        },
                        new
                        {
                            BookId = 3,
                            Author = "Harper Lee",
                            CategoryId = 2,
                            Description = "To Kill a Mockingbird is a novel by Harper Lee published in 1960. Instantly successful, widely read in high schools and middle schools in the United States, it has become a classic of modern American literature, winning the Pulitzer Prize.",
                            ImageUrl = "",
                            ListPrice = 14.99,
                            Price = 12.99,
                            Price3 = 11.99,
                            Price5 = 10.99,
                            Title = "To Kill a Mockingbird"
                        },
                        new
                        {
                            BookId = 4,
                            Author = "George Orwell",
                            CategoryId = 2,
                            Description = "1984 is a dystopian social science fiction novel by English novelist George Orwell. It was published on 8 June 1949 by Secker & Warburg as Orwell's ninth and final book completed in his lifetime.",
                            ImageUrl = "",
                            ListPrice = 16.989999999999998,
                            Price = 14.99,
                            Price3 = 13.99,
                            Price5 = 12.99,
                            Title = "1984"
                        },
                        new
                        {
                            BookId = 5,
                            Author = "Aldous Huxley",
                            CategoryId = 2,
                            Description = "Brave New World is a dystopian social science fiction novel by English author Aldous Huxley, written in 1931 and published in 1932. Largely set in a futuristic World State, whose citizens are environmentally engineered into an intelligence-based social hierarchy.",
                            ImageUrl = "",
                            ListPrice = 18.989999999999998,
                            Price = 16.989999999999998,
                            Price3 = 15.99,
                            Price5 = 14.99,
                            Title = "Brave New World"
                        },
                        new
                        {
                            BookId = 6,
                            Author = "J. R. R. Tolkien",
                            CategoryId = 3,
                            Description = "The Lord of the Rings is an epic high-fantasy novel by English author and scholar J. R. R. Tolkien. Set in Middle-earth, the world at some distant time in the past, the story began as a sequel to Tolkien's 1937 children's book The Hobbit, but eventually developed into a much larger work.",
                            ImageUrl = "",
                            ListPrice = 20.989999999999998,
                            Price = 18.989999999999998,
                            Price3 = 17.989999999999998,
                            Price5 = 16.989999999999998,
                            Title = "The Lord of the Rings"
                        });
                });

            modelBuilder.Entity("BookLink.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"));

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CategoryId");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            CategoryId = 1,
                            CategoryName = "Literature"
                        },
                        new
                        {
                            CategoryId = 2,
                            CategoryName = "History"
                        },
                        new
                        {
                            CategoryId = 3,
                            CategoryName = "Economics"
                        },
                        new
                        {
                            CategoryId = 4,
                            CategoryName = "Medicine & Health"
                        },
                        new
                        {
                            CategoryId = 5,
                            CategoryName = "Religions & Beliefs"
                        },
                        new
                        {
                            CategoryId = 6,
                            CategoryName = "Self-Development"
                        },
                        new
                        {
                            CategoryId = 7,
                            CategoryName = "Science Fiction & Fantasy"
                        },
                        new
                        {
                            CategoryId = 8,
                            CategoryName = "Philosophy"
                        });
                });

            modelBuilder.Entity("BookLink.Models.Book", b =>
                {
                    b.HasOne("BookLink.Models.Category", "BookCategory")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BookCategory");
                });
#pragma warning restore 612, 618
        }
    }
}
