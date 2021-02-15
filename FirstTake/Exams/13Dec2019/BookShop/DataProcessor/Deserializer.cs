namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(List<BookImportDTO>), new XmlRootAttribute("Books"));
            var books = (List<BookImportDTO>)serializer.Deserialize(new StringReader(xmlString));
            var sb = new StringBuilder();
            foreach (var book in books)
            {
                if (!IsValid(book))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                DateTime publishedOn;
                if (!DateTime.TryParseExact(book.PublishedOn, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,out publishedOn))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var bookToAdd = new Book()
                {
                    Name = book.Name,
                    Genre = (Genre)book.Genre,
                    Pages = book.Pages,
                    Price = book.Price,
                    PublishedOn = publishedOn
                };

                context.Books.Add(bookToAdd);
                sb.AppendLine(string.Format(SuccessfullyImportedBook, book.Name, book.Price));
            }
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            var authors = JsonConvert.DeserializeObject<List<AuthorImportDTO>>(jsonString);
            var sb = new StringBuilder();
            foreach (var author in authors)
            {
                if (!IsValid(author) || context.Authors.Any(x => x.Email == author.Email))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var authorToAdd = new Author()
                {
                    Email = author.Email,
                    FirstName = author.FirstName,
                    LastName = author.LastName,
                    Phone = author.Phone
                };
                var books = new List<Book>();
                foreach (var book in author.Books)
                {
                    var currentBook = context.Books.FirstOrDefault(x => x.Id == book.Id);
                    if (currentBook == null)
                    {
                        continue;
                    }
                    books.Add(currentBook);
                }
                if (books.Count() == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                foreach (var item in books)
                {
                    authorToAdd.AuthorsBooks.Add(new AuthorBook()
                    {
                        Book = item,
                        Author = authorToAdd
                    });
                }
                context.Authors.Add(authorToAdd);
                sb.AppendLine(string.Format(SuccessfullyImportedAuthor, $"{authorToAdd.FirstName} {authorToAdd.LastName}", authorToAdd.AuthorsBooks.Count()));
                context.SaveChanges();
            }
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}