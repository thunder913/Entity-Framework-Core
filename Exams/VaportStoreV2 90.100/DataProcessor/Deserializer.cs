namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
	{
		static string ErrorMessage = "Invalid Data";
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
			var sb = new StringBuilder();
			var games = JsonConvert.DeserializeObject<List<GameImportDTO>>(jsonString);
            foreach (var game in games)
            {
                if (!IsValid(game) || !game.Tags.Any(x=>!string.IsNullOrWhiteSpace(x)))
                {
					sb.AppendLine(ErrorMessage);
					continue;
                }

				DateTime ReleaseDate;

                if (!DateTime.TryParseExact(game.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out ReleaseDate))
                {
					sb.AppendLine(ErrorMessage);
					continue;
                }
				Developer developer = context.Developers.FirstOrDefault(x=>x.Name == game.DeveloperName);
                if (developer == null)
                {
					developer = new Developer() { Name = game.DeveloperName };
					context.Developers.Add(developer);
                }
				Genre genre = context.Genres.FirstOrDefault(x => x.Name == game.GenreName);
                if (genre == null)
                {
					genre = new Genre() { Name = game.GenreName };
					context.Genres.Add(genre);
                }
				var currentGame = new Game()
				{
					ReleaseDate = ReleaseDate,
					Name = game.GameName,
					Price = game.Price,
					Developer = developer,
					Genre = genre
				};

                foreach (var tag in game.Tags)
                {
					if (!string.IsNullOrWhiteSpace(tag))
					{
						var tagFromContext = context.Tags.FirstOrDefault(x => x.Name == tag);
						if (tagFromContext == null)
						{
							tagFromContext = new Tag() { Name = tag };
							context.Tags.Add(tagFromContext);
						}
						currentGame.GameTags.Add(new GameTag() { Game = currentGame, Tag = tagFromContext });
					}
				
				}
				context.Games.Add(currentGame);
				context.SaveChanges();
				sb.AppendLine($"Added {game.GameName} ({game.GenreName}) with {game.Tags.Count()} tags");
            }
			return sb.ToString().TrimEnd();
		}

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			var sb = new StringBuilder();
			var usersToAdd = new List<User>();
			var users = JsonConvert.DeserializeObject<UserImportDTO[]>(jsonString);
            foreach (var user in users)
            {
                if (!IsValid(user) || user.Cards.Any(x=>!IsValid(x)))
                {
					sb.AppendLine(ErrorMessage);
					continue;
                }
				var userToAdd = new User()
				{
					Age = user.Age,
					Email = user.Email,
					FullName = user.FullName,
					Username = user.Username
				};

				var cards = new List<Card>();
				bool NotValid = false;
                foreach (var card in user.Cards)
                {
					CardType cardType;
					if (!Enum.TryParse<CardType>(card.Type, true, out cardType ))
					{
						NotValid = true;
						sb.AppendLine(ErrorMessage);
						continue;
					}
					var cardToImport = new Card()
					{
						Cvc = card.Cvc,
						Number = card.Number,
						Type = cardType,
						User = userToAdd
					};
					context.Cards.Add(cardToImport);
					userToAdd.Cards.Add(cardToImport);
                }

                if (NotValid)
                {
					continue;
                }
				context.Users.Add(userToAdd);
				sb.AppendLine($"Imported {userToAdd.Username} with {userToAdd.Cards.Count()} cards");
            }

			context.SaveChanges();
			return sb.ToString().TrimEnd();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			var serializer = new XmlSerializer(typeof(PurchaseImportDTO[]), new XmlRootAttribute("Purchases"));
			var purchases = (PurchaseImportDTO[])serializer.Deserialize(new StringReader(xmlString));
			var purchasesToImport = new List<Purchase>();
			var sb = new StringBuilder();
            foreach (var purchase in purchases)
            {
                if (!IsValid(purchase))
                {
					sb.AppendLine(ErrorMessage);
					continue;
                }
				PurchaseType purchaseType;
                if (!Enum.TryParse<PurchaseType>(purchase.Type, out purchaseType))
                {
					sb.AppendLine(ErrorMessage);
					continue;
                }
				var card = context.Cards.FirstOrDefault(x => x.Number == purchase.CardNumber);
				var game = context.Games.FirstOrDefault(x => x.Name == purchase.Title);

				DateTime Date;
				var validDate = DateTime.TryParseExact(purchase.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out Date);
				if (card == null || game == null || validDate == false)
                {
					sb.AppendLine(ErrorMessage);
					continue;
                }

				var currentPurchase = new Purchase()
				{
					Card = card,
					Game = game,
					Date = Date,
					ProductKey = purchase.ProductKey,
					Type = purchaseType
				};

				var userName = context.Users.First(x => x.Cards.Any(c => c.Number == purchase.CardNumber)).Username;
				purchasesToImport.Add(currentPurchase);
				sb.AppendLine($"Imported {purchase.Title} for {userName}");
			}
			context.Purchases.AddRange(purchasesToImport);
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