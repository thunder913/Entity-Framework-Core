namespace VaporStore.DataProcessor
{
	using System;
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

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
			var genres =
				context
				.Genres
				//Just add .ToArray() to work in judge ;x
				.ToArray()
				.Where(g => genreNames.Contains(g.Name))
				.Select(g => new
				{
					Id=g.Id,
					Genre = g.Name,
					Games = g.Games
					.Where(ga=>ga.Purchases.Any())
					.Select(ga => new
					{
						ga.Id,
						Title = ga.Name,
						Developer = ga.Developer.Name,
						Tags = String.Join(", ", ga.GameTags.Select(gt => gt.Tag.Name).ToArray()),
						Players = ga.Purchases.Count
					})
					.OrderByDescending(ga=>ga.Players)
					.ThenBy(ga=>ga.Id)
					.ToArray(),
					TotalPlayers = g.Games.Sum(ga=>ga.Purchases.Count)
				})
                .OrderByDescending(g => g.TotalPlayers)
                .ThenBy(g => g.Id)
                .ToArray();

			var jsonText = JsonConvert.SerializeObject(genres, Newtonsoft.Json.Formatting.Indented);
			return jsonText;
		}
		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{

			var storeTypeEnum = (PurchaseType) Enum.Parse(typeof(PurchaseType), storeType);

			var users =
				context
				.Users
				.ToArray()
				.Where(u=>u.Cards.Any(x=>x.Purchases.Any(p=>p.Type==storeTypeEnum)))
				.Select(u => new UserExportDto()
				{
					UserName = u.Username,
					Purchases = context
							.Purchases
							.Where(p => p.Card.User.Id == u.Id && p.Type == storeTypeEnum)
							.OrderBy(x=>x.Date)
							.Select(p => new PurchaseExportDto()
							{
								Card = p.Card.Number,
								Cvc = p.Card.Cvc,
								Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
								Game = new GameExportDto()
								{
									Genre = p.Game.Genre.Name,
									Price = p.Game.Price,
									Title = p.Game.Name
								}
							})
							.ToArray(),
					TotalSpent = context.Purchases.Where(pur => pur.Card.User.Id == u.Id && pur.Type==storeTypeEnum).Sum(x => x.Game.Price)
				})
				.OrderByDescending(x=>x.TotalSpent)
				.ThenBy(x=>x.UserName)
				.ToArray();

			var serialzier = new XmlSerializer(typeof(UserExportDto[]), new XmlRootAttribute("Users"));
			var sb = new StringBuilder();
			serialzier.Serialize(new StringWriter(sb), users, new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") }));
			return sb.ToString().TrimEnd();
		}
	}
}