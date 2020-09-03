namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.DataProcessor.Dto.Export;
	using System.Xml;
	using System.Xml.Serialization;
    using System.Text;
    using System.IO;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
			var games = context
				.Genres
				.Where(x => genreNames.Contains(x.Name))
				.Select(x => new GenreExportDTO()
				{
					Id = x.Id,
					Genre = x.Name,
					Games = x.Games
						.Where(z => z.Purchases.Any())
						.Select(y => new GameExportDTO()
						{
							Developer = y.Developer.Name,
							Id = y.Id,
							Title = y.Name,
							Players = y.Purchases.Count(),
							Tags = String.Join(", ", y.GameTags.Select(z => z.Tag.Name).ToArray()),
						})
							.OrderByDescending(x => x.Players)
							.ThenBy(x => x.Id)
							.ToList()
				}).ToList();

			var jsonText = JsonConvert.SerializeObject(games);

			return jsonText;
		}

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
			var users = context.Users
				.Where(u => u.Cards.Any(y => y.Purchases.Any()))
				.Select(u => new ExportUserPurchaseDTO()
				{
					Username = u.Username,
					Purchases = context.Purchases
							.Where(p => p.Card.UserId == u.Id)
							.OrderBy(p=>p.Date)
							.Select(p => new PurchaseExportDTO()
							{
								Card = p.Card.Number,
								Cvc = p.Card.Cvc,
								Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
								Game = new GamePurchaseExportDTO()
								{
									Genre = p.Game.Genre.Name,
									Price = p.Game.Price,
									Title = p.Game.Name
								}
							})
							.ToList()
				})
				.OrderByDescending(x=>x.Purchases.Sum(y=>y.Game.Price))
				.ThenBy(x=>x.Username)
				.ToList();
				var sb = new StringBuilder();
			var serializer = new XmlSerializer(typeof(List<ExportUserPurchaseDTO>), new XmlRootAttribute("Users"));
			XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

			serializer.Serialize(new StringWriter(sb), users, xmlNamespaces);
			return sb.ToString().TrimEnd();
		}
	}
}