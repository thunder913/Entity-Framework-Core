namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            var context = new BookShopContext();
            var result = RemoveBooks(context);
            Console.WriteLine(result);
        }
        //Problem 1
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            command = myTI.ToTitleCase(command);
            AgeRestriction ageRestriction = (AgeRestriction)Enum.Parse(typeof(AgeRestriction), command);
            var books = context
                .Books
                .Where(x => x.AgeRestriction == ageRestriction)
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToList();
            var sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine(book.ToString());
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 2

        public static string GetGoldenBooks(BookShopContext context)
        {
            var gold = EditionType.Gold;

            var books = context.Books
                .Where(x => x.EditionType == gold && x.Copies < 5000)
                .OrderBy(x => x.BookId)
                .Select(x => x.Title).ToList();

            var sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine(book.ToString());
            }
            return sb.ToString().TrimEnd();
        }

        //Problem 3
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.Price > 40)
                .OrderByDescending(x => x.Price)
                .Select(x => new { x.Title, x.Price })
                .ToList();
            var sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        //Problem 4

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books.Where(x => x.ReleaseDate.Value.Year != year)
                .OrderBy(x => x.BookId)
                .Select(x => x.Title)
                .ToList();

            var sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine(book.ToString());
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 5

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.ToUpper().Split().ToList();

            var books = context.Books
                .Where(b => b.BookCategories
                    .Any(bc => categories
                        .Contains(bc.Category.Name.ToUpper())))
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToList();


            return string.Join(Environment.NewLine, books);
        }

        //PROBLEM 6

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var dateSplitted = date.Split('-').Select(int.Parse).ToList();
            var dateTime = new DateTime(dateSplitted[2], dateSplitted[1], dateSplitted[0]);
            var books = context.Books
                .Where(x => x.ReleaseDate < dateTime)
                .OrderByDescending(x => x.ReleaseDate)
                .Select(x => new { x.Title, x.EditionType, x.Price })
                .ToList();

            var sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        //Problem 7
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                    .Where(x => x.FirstName.EndsWith(input))
                    .Select(x => new { Name = x.FirstName + " " + x.LastName })
                    .OrderBy(x => x.Name)
                    .ToList();

            var sb = new StringBuilder();
            foreach (var author in authors)
            {
                sb.AppendLine(author.Name);
            }
            return sb.ToString().TrimEnd();
        }

        //Problem 8
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToList();

            return string.Join(Environment.NewLine, books);
        }

        //Problem 9
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(x => x.BookId)
                .Select(x => new { AuthorName = x.Author.FirstName + " " + x.Author.LastName, x.Title })
                .ToList();

            var sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.AuthorName})");
            }
            return sb.ToString().TrimEnd();
        }
        //Problem 10

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books.Where(x => x.Title.Count() > lengthCheck).Count();

        }
        //Problem 11
        public static string CountCopiesByAuthor(BookShopContext context)
        {

            var authorCopies = context.Authors
                .Select(x => new
                {
                    Copies = x.Books
                        .Select(x => x.Copies).Sum(),
                    AuthorName = x.FirstName + " " + x.LastName
                })
                .OrderByDescending(x => x.Copies)
                .ToList();

            var sb = new StringBuilder();
            foreach (var authorCopy in authorCopies)
            {
                sb.AppendLine($"{authorCopy.AuthorName} - {authorCopy.Copies}");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 12
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var profitPerBook = context.Categories
                .Select(x => new { x.Name, Profit = x.CategoryBooks.Select(x => x.Book.Price * x.Book.Copies).Sum() })
                .OrderByDescending(x => x.Profit)
                .ToList();


            var sb = new StringBuilder();
            foreach (var profitBook in profitPerBook)
            {
                sb.AppendLine($"{profitBook.Name} ${profitBook.Profit}");
            }
            return sb.ToString().TrimEnd();
        }
        //Problem 13

        public static string GetMostRecentBooks(BookShopContext context) 
        {
            var categories = context.Categories
                .Select(x => new
                {
                    CategoryName = x.Name,
                    Books = x.CategoryBooks
                        .Select(x => new { x.Book.Title, x.Book.ReleaseDate })
                        .OrderByDescending(x => x.ReleaseDate)
                        .Take(3)
                })
                .OrderBy(x => x.CategoryName)
                .ToList();

            var sb = new StringBuilder();
            foreach (var item in categories)
            {
                sb.AppendLine($"--{item.CategoryName}");
                foreach (var book in item.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();

        }
        //Problem 14
        public static void IncreasePrices(BookShopContext context) 
        {
            var books = context.Books.Where(x => x.ReleaseDate.Value.Year < 2010).ToList();

            for (int i = 0; i < books.Count(); i++)
            {
                books[i].Price += 5;
            }

            context.SaveChanges();
                
        }

        //Problem 15 (50/100 because of Judge bug)
        public static int RemoveBooks(BookShopContext context) 
        {
            var books = context.Books.Where(x => x.Copies < 4200).ToList();
            var removed = books.Count();
            foreach (var book in books)
            {
                context.Remove(book);
            }
            context.SaveChanges();

            return removed;

        }
    }
}
