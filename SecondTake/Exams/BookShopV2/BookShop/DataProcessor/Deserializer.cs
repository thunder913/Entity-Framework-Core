namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
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
            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(BookImportDto[]), new XmlRootAttribute("Books"));
            var books = (BookImportDto[]) serializer.Deserialize(new StringReader(xmlString));

            foreach (var book in books)
            {
                if (IsValid(book) && Enum.IsDefined(typeof(Genre), book.Genre))
                {
                    var bookToImport = new Book()
                    {
                        Name = book.Name,
                        Genre = (Genre) book.Genre,
                    Pages = book.Pages,
                        Price = book.Price,
                        PublishedOn = DateTime.ParseExact(book.PublishedOn, "MM/dd/yyyy", CultureInfo.InvariantCulture)
                    };
                    context.Books.Add(bookToImport);
                    sb.AppendLine(String.Format(SuccessfullyImportedBook, bookToImport.Name, bookToImport.Price));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            var authors = JsonConvert.DeserializeObject<AuthorImportDto[]>(jsonString);
            var sb = new StringBuilder();
            var hasError = false;
            foreach (var auth in authors)
            {
                if (IsValid(auth))
                {
                    if (!context.Authors.Any(x=>x.Email==auth.Email))
                    {
                        var books = new List<BookImportDtoId>();
                        foreach (var bookId in auth.Books)
                        {
                            if (bookId.Id!=null && context.Books.Any(x=>x.Id==bookId.Id))
                            {
                                books.Add(bookId);
                            }
                        }

                        if (books.Any())
                        {
                            var author = new Author()
                            {
                                Email = auth.Email,
                                FirstName = auth.FirstName,
                                LastName = auth.LastName,
                                Phone = auth.Phone,
                            };

                            context.Authors.Add(author);

                            foreach (var authorBook in books)
                            {
                                context.AuthorsBooks.Add(new AuthorBook()
                                {
                                    Author = author,
                                    BookId = (int)authorBook.Id
                                });
                            }

                            context.SaveChanges();
                            sb.AppendLine(string.Format(SuccessfullyImportedAuthor, author.FirstName + " " + author.LastName, books.Count()));
                        }
                        else
                        {
                            hasError = true;
                        }
                    }
                    else
                    {
                        hasError = true;
                    }
                }
                else
                {
                    hasError = true;
                }

                if (hasError)
                {
                    sb.AppendLine(ErrorMessage);
                    hasError = false;
                }
            }
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