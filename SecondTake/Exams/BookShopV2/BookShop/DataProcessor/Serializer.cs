namespace BookShop.DataProcessor
{
    using System;
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
            var authors =
                context
                .Authors
                .ToArray()
                .Select(a => new
                {
                    AuthorName = a.FirstName + " " + a.LastName,
                    Books = context.AuthorsBooks.Where(y => y.AuthorId == a.Id)
                    .Select(ab => new
                    {
                        BookName = ab.Book.Name,
                        BookPrice = $"{ab.Book.Price:f2}"
                    })
                    .OrderByDescending(x => decimal.Parse(x.BookPrice))
                    .ToArray()
                })
                .OrderByDescending(x => x.Books.Count())
                .ThenBy(x => x.AuthorName)
                .ToArray();

            var jsonText = JsonConvert.SerializeObject(authors, Formatting.Indented);
            return jsonText;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var books =
                context
                .Books
                .ToArray()
                .Where(x => x.PublishedOn < date && x.Genre==Genre.Science)
                 .OrderByDescending(x => x.Pages)
                .ThenByDescending(x => x.PublishedOn)
                .Take(10)

                .Select(b => new BookExportDto()
                {
                    Name = b.Name,
                    Date = b.PublishedOn.ToString("d", CultureInfo.InvariantCulture),
                    Pages = b.Pages
                })
                .ToArray();

            var serializer = new XmlSerializer(typeof(BookExportDto[]), new XmlRootAttribute("Books"));
            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), books, new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") }));

            return sb.ToString().TrimEnd();
        }
    }
}