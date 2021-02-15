namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto.Import;
    using Data.Models.Enums;
    using System.Xml;
    using System.Xml.Serialization;
    using System.IO;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var games = JsonConvert.DeserializeObject<List<GameDTO>>(jsonString);
            var regexReleaseDate = new Regex(@"^[\d]{4}-[\d]{2}-[\d]{2}$");
            var sb = new StringBuilder();

            foreach (var game in games)
            {

                if (game.Name == null || game.ReleaseDate == null
                    || game.Developer == null || game.Genre == null
                    || game.Tags.Count() == 0 || game.Price < 0
                    || !regexReleaseDate.IsMatch(game.ReleaseDate))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                if (!context.Developers.Any(x => x.Name == game.Developer))
                {
                    context.Developers.Add(new Developer() { Name = game.Developer });
                }
                if (!context.Genres.Any(x => x.Name == game.Genre))
                {
                    context.Genres.Add(new Genre() { Name = game.Genre });
                }
                context.SaveChanges();
                var gameTags = new List<GameTag>();
                var tags = new List<Tag>();
                Game currentGame = new Game()
                {
                    Name = game.Name,
                    ReleaseDate = DateTime.Parse(game.ReleaseDate, CultureInfo.InvariantCulture),
                    Price = game.Price,
                    DeveloperId = context.Developers.First(x => x.Name == game.Developer).Id,
                    GenreId = context.Genres.First(x => x.Name == game.Genre).Id
                };

                foreach (var tag in game.Tags)
                {
                    if (!context.Tags.Any(x => x.Name == tag))
                    {
                        tags.Add(new Tag() { Name = tag });
                    }
                }
                foreach (var tag in tags)
                {
                    gameTags.Add(new GameTag()
                    {
                        Game = currentGame,
                        Tag = tag
                    });
                }
                context.GameTags.AddRange(gameTags);
                currentGame.GameTags = gameTags;
                context.Games.Add(currentGame);


                sb.AppendLine($"Added {game.Name} ({game.Genre}) with {game.Tags.Count()} tags");
            }
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var users = JsonConvert.DeserializeObject<List<UserDTO>>(jsonString);
            var sb = new StringBuilder();
            var userfullNameRegex = new Regex(@"^[A-Z][a-z]+ [A-Z][a-z]+$");
            var cardNumberRegex = new Regex(@"^[\d]{4} [\d]{4} [\d]{4} [\d]{4}$");
            var cardCvcRegex = new Regex(@"^[\d]{3}$");
            foreach (var user in users)
            {
                if (user.Username.Length < 3 || user.Username.Length > 20 || !userfullNameRegex.IsMatch(user.FullName)
                    || user.Age < 3 || user.Age > 103 || string.IsNullOrEmpty(user.Email) || user.Cards.Count() == 0
                    || !user.Cards.All(x => cardNumberRegex.IsMatch(x.Number) && cardCvcRegex.IsMatch(x.Cvc)
                    || !user.Cards.All(x => x.Type == CardType.Credit || x.Type == CardType.Debit)))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var currentUser = new User()
                {
                    Age = user.Age,
                    Email = user.Email,
                    FullName = user.FullName,
                    Username = user.Username
                };
                var cards = new List<Card>();
                foreach (var card in user.Cards)
                {
                    var currentCard = new Card()
                    {
                        Cvc = card.Cvc,
                        Number = card.Number,
                        Type = card.Type,
                        User = currentUser
                    };
                    if (!context.Cards.Any(x => x.Number == card.Number && x.Cvc == card.Cvc && x.Type == card.Type))
                    {
                        context.Cards.Add(currentCard);
                    }
                    cards.Add(currentCard);
                    currentUser.Cards.Add(currentCard);
                }
                context.Add(currentUser);
                sb.AppendLine($"Imported {currentUser.Username} with {currentUser.Cards.Count()} cards");
            }
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(PurchaseDTO[]), new XmlRootAttribute("Purchases"));
            var dateRegex = new Regex(@"^[\d]{2}\/[\d]{2}\/[\d]{4} [\d]{2}:[\d]{2}$");
            var productKeyRegex = new Regex(@"^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$");
            var purchases = (PurchaseDTO[])serializer.Deserialize(new StringReader(xmlString));
            var sb = new StringBuilder();
            foreach (var purchase in purchases)
            {
                if (!context.Games.Any(x => x.Name == purchase.GameName)
                    || !(purchase.Type == PurchaseType.Digital || purchase.Type == PurchaseType.Retail)
                    || !productKeyRegex.IsMatch(purchase.ProductKey)
                    || !context.Cards.Any(x => x.Number == purchase.Card)
                    || !dateRegex.IsMatch(purchase.Date))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var currentPurchase = new Purchase()
                {
                    Card = context.Cards.First(x => x.Number == purchase.Card),
                    Date = DateTime.ParseExact(purchase.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    Type = purchase.Type,
                    ProductKey = purchase.ProductKey,
                    Game = context.Games.First(x=>x.Name==purchase.GameName)
                };

                context.Purchases.Add(currentPurchase);
                context.SaveChanges();
                sb.AppendLine($"Imported {purchase.GameName} for {context.Users.First(x => x.Cards.Any(y => y.Number == purchase.Card)).Username}");
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