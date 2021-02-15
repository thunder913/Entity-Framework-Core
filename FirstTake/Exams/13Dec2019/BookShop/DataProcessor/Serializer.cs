namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var authors = context.Authors
                .ToList()
                    .Select(x => new AuthorExportDTO()
                    {
                        AuthorName = x.FirstName + " " + x.LastName,
                        Books = context.AuthorsBooks.Where(y=>y.AuthorId == x.Id).Select(ab => new BookExportDTO()
                        {
                            BookName = ab.Book.Name,
                            BookPrice = $"{ab.Book.Price:f2}"
                        })
                        .OrderByDescending(b => decimal.Parse(b.BookPrice))
                        .ToList()
                    })
                    .OrderByDescending(x => x.Books.Count())
                    .ThenBy(x=>x.AuthorName)
                    .ToList();

            var jsonTest = JsonConvert.SerializeObject(authors, Formatting.Indented);
            return jsonTest;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var books = context.Books
                .Where(x => x.PublishedOn < date && x.Genre == Genre.Science)
                .ToArray()
                .Select(b => new BookXMLExportDTO()
                {
                    Name = b.Name,
                    Date = b.PublishedOn.ToString(@"MM/dd/yyyy"),
                    Pages = b.Pages
                })
                .OrderByDescending(x => x.Pages)
                .ThenByDescending(x => x.Date)
                .Take(10)
                .ToList();

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(List<BookXMLExportDTO>), new XmlRootAttribute("Books"));
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), books, xmlNamespaces);
            return sb.ToString().TrimEnd();
        }
    }
}