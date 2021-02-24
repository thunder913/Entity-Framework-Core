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
		public static string ErrorMessage = "Invalid Data";
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
			var hasError = false;
			var sb = new StringBuilder();
			var games = JsonConvert.DeserializeObject<GameImportDto[]>(jsonString);
            foreach (var game in games)
            {
                if (IsValid(game) && game.Tags.Any())
                {
					Developer developer = context.Developers.FirstOrDefault(x => x.Name == game.Developer);
					Genre genre = context.Genres.FirstOrDefault(x => x.Name == game.Genre);
					List<Tag> tags = new List<Tag>();

					for(int i = 0; i<game.Tags.Count(); i++)
					{
                        if (String.IsNullOrWhiteSpace(game.Tags[i]))
                        {
							hasError = true;
							break;
                        }
                        else
                        {
                            if (context.Tags.Any(x=>x.Name==game.Tags[i]))
                            {
								tags.Add(context.Tags.FirstOrDefault(x => x.Name == game.Tags[i]));
                            }
                            else
                            {
								var tagToAdd = new Tag()
								{
									Name = game.Tags[i]
								};
								context.Tags.Add(tagToAdd);
								tags.Add(tagToAdd);
                            }
                        }
					}
					if (!hasError)
					{
						if (developer == null)
						{
							developer = new Developer()
							{
								Name = game.Developer
							};
							context.Developers.Add(developer);
						}
						if (genre == null)
						{
							genre = new Genre()
							{
								Name = game.Genre
							};
							context.Genres.Add(genre);
						}



						var gameToAdd = new Game()
						{
							Name = game.Name,
							ReleaseDate = DateTime.ParseExact(game.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
							Developer = developer,
							Genre = genre,
							Price = game.Price
						};

						var gameTags = new List<GameTag>();
                        foreach (var tag in tags)
                        {
							gameTags.Add(new GameTag()
							{
								Game = gameToAdd,
								Tag = tag
							});
                        }

						context.Games.Add(gameToAdd);
						context.GameTags.AddRange(gameTags);
						context.SaveChanges();
						sb.AppendLine($"Added {gameToAdd.Name} ({gameToAdd.Genre.Name}) with {gameTags.Count()} tags");
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

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			bool hasError = false;
			var users = JsonConvert.DeserializeObject<UserImportDto[]>(jsonString);
			var sb = new StringBuilder();
			foreach (var user in users)
			{
				if (IsValid(user))
				{
					for (int i = 0; i < user.Cards.Count(); i++)
					{
						var card = user.Cards[i];
						if (!IsValid(card))
						{
							hasError = true;
							break;
						}
					}

					if (!hasError)
					{
						List<Card> cards = new List<Card>();
						foreach (var card in user.Cards)
						{
							var newCard = new Card()
							{
								Number = card.Number,
								Cvc = card.Cvc,
								Type = card.Type
							};
							cards.Add(newCard);
						}

						context.Cards.AddRange(cards);

						var userToAdd = new User()
						{
							FullName = user.FullName,
							Age = user.Age,
							Email = user.Email,
							Username = user.Username,
							Cards = cards
						};
						context.Users.Add(userToAdd);
						sb.AppendLine($"Imported {userToAdd.Username} with {userToAdd.Cards.Count} cards");
					}
				}
				else
				{
					hasError = true;
				}

				if (hasError)
				{
					hasError = false;
					sb.AppendLine(ErrorMessage);
				} 
			}

			context.SaveChanges();
			return sb.ToString().TrimEnd();

		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			var serializer = new XmlSerializer(typeof(PurchaseImportDto[]), new XmlRootAttribute("Purchases"));
			var purchases = (PurchaseImportDto[])serializer.Deserialize(new StringReader(xmlString));
			var sb = new StringBuilder();
			var hasError = false;
            foreach (var purchase in purchases)
            {
				var productType = (PurchaseType)Enum.Parse(typeof(PurchaseType), purchase.Type);
                if (IsValid(purchase))
                {
					var card = context.Cards.Where(x => x.Number == purchase.Card).FirstOrDefault();
					var game = context.Games.Where(x => x.Name == purchase.GameName).FirstOrDefault();
					var date = DateTime.ParseExact(purchase.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

					var newPurchase = new Purchase()
					{
						ProductKey = purchase.ProductKey,
						Date = date,
						Card = card,
						Game = game,
						Type = productType
					};
					var userName = context.Users.Where(x => x.Cards.Any(y => y.Number == card.Number)).Select(x=>x.Username).FirstOrDefault();
					context.Add(newPurchase);
					sb.AppendLine($"Imported {game.Name} for {userName}");

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