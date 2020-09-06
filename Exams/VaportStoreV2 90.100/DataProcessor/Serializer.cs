namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Export;
    using Formatting = Newtonsoft.Json.Formatting;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var games = context.Genres
                            .ToArray()
                            .Where(x => genreNames.Contains(x.Name))
                            .Select(x => new GenreExportDTO
                            {
                                Id = x.Id,
                                Genre = x.Name,
                                Games = x.Games.Where(g => g.Purchases.Any())
                                        .Select(g => new GameExportDTO()
                                        {
                                            Id = g.Id,
                                            Developer = g.Developer.Name,
                                            Tags = string.Join(", ", g.GameTags.Select(y => y.Tag.Name).ToArray()),
                                            Title = g.Name,
                                            Players = g.Purchases.Count()
                                        })
                                        .OrderByDescending(x => x.Players)
                                        .ThenBy(x => x.Id)
                                        .ToList(),
                                TotalPlayers = context.Purchases.Where(y => y.Game.Genre.Name == x.Name).Count()
                            })
                            .OrderByDescending(x => x.TotalPlayers)
                            .ThenBy(x => x.Id)
                            .ToList();
            var jsonText = JsonConvert.SerializeObject(games, Formatting.Indented);

            return jsonText;
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            var purchaseType = (PurchaseType)Enum.Parse(typeof(PurchaseType), storeType);

            var users = context.Users
                            .Where(x => x.Cards.Any(c => c.Purchases.Any(y=>y.Type == purchaseType)))
                            .ToArray()
                            .Select(u => new UserExportDTO()
                            {
                                Username = u.Username,
                                Purchases = context.Purchases.Where(p => p.Card.User == u && p.Type == purchaseType)
                                        .OrderBy(p=>p.Date)
                                         .Select(p => new PurchaseExportDTO()
                                         {
                                             Card = p.Card.Number,
                                             Cvc = p.Card.Cvc,
                                             Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                                             Game = new GamePurchaseDTO()
                                             {
                                                 Genre = p.Game.Genre.Name,
                                                 Price = p.Game.Price,
                                                 Title = p.Game.Name
                                             }
                                         })
                                         .ToList()
                            }).ToList();

            for (int i = 0; i < users.Count(); i++)
            {
                users[i].TotalSpent = users[i].Purchases.Sum(x => x.Game.Price);
            }

            users = users.OrderByDescending(x => x.TotalSpent).ThenBy(x => x.Username).ToList();

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(List<UserExportDTO>), new XmlRootAttribute("Users"));
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), users, xmlNamespaces);
            return sb.ToString().TrimEnd();
        }
    }
}