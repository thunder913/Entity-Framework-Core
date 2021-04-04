using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.Dto;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();
            //ImportUsers(context, "Datasets/users.json");
            //ImportProducts(context, "Datasets/products.json");
            //ImportCategories(context, "Datasets/categories.json");
            //ImportCategoryProducts(context, "Datasets/categories-products.json");
            Console.WriteLine(GetUsersWithProducts(context));
        }
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var jsonText = File.ReadAllText(inputJson);
            var users = JsonConvert.DeserializeObject<User[]>(jsonText);
            context.Users.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Length}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var jsonText = File.ReadAllText(inputJson);
            var products = JsonConvert.DeserializeObject<Product[]>(jsonText);
            context.Products.AddRange(products);
            context.SaveChanges();
            return $"Successfully imported {products.Length}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var jsonText = File.ReadAllText(inputJson);
            var categories = JsonConvert.DeserializeObject<Category[]>(jsonText
                                    , new JsonSerializerSettings
                                    {
                                        NullValueHandling = NullValueHandling.Ignore,
                                    })
                                    .Where(x => x.Name != null)
                                    .ToArray();
            context.Categories.AddRange(categories);
            context.SaveChanges();
            return $"Successfully imported {categories.Length}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var jsonText = File.ReadAllText(inputJson);
            var categoryProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(jsonText);
            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();
            return $"Successfully imported {categoryProducts.Length}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products =
                    context
                    .Products
                        .Where(p => p.Price >= 500 && p.Price <= 1000)
                        .OrderBy(x => x.Price)
                        .Select(x => new
                        {
                            x.Name,
                            x.Price,
                            Seller = x.Seller.FirstName + " " + x.Seller.LastName
                        })
                        .ToList();

            var jsonText = JsonConvert.SerializeObject(products, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                }
            });
            return jsonText;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users =
                    context
                    .Users
                    .ToList()
                    .Where(x => x.ProductsSold.Any(y => y.Buyer != null))
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .Select(x => new
                    {
                        firstName = x.FirstName,
                        lastName = x.LastName,
                        soldProducts = x.ProductsSold
                        .Where(y => y.Buyer != null)
                        .Select(y => new
                        {
                            name = y.Name,
                            price = y.Price,
                            buyerFirstName = y.Buyer.FirstName,
                            buyerLastName = y.Buyer.LastName
                        })
                        .ToList()
                    })
                    .ToList();

            var jsonText = JsonConvert.SerializeObject(users, Formatting.Indented);
            return jsonText;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories =
                    context
                    .Categories
                    .OrderByDescending(x => x.CategoryProducts.Count())
                    .Select(x => new
                    {
                        Category = x.Name,
                        ProductsCount = x.CategoryProducts.Count(),
                        AveragePrice = $"{x.CategoryProducts.Average(y => y.Product.Price):f2}",
                        TotalRevenue = $"{x.CategoryProducts.Sum(y => y.Product.Price):f2}"
                    })
                    .ToList();

            var jsonText = JsonConvert.SerializeObject(categories, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            return jsonText;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Include(x => x.ProductsSold)
                .ToList()
                .Where(x => x.ProductsSold.Any(y => y.Buyer != null))
                .Select(x => new
                {
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Age = x.Age,
                        SoldProducts = new
                        {
                            Count = x.ProductsSold.Where(y => y.Buyer != null).Count(),
                            Products = x.ProductsSold.Where(y => y.Buyer != null).Select(y => new
                            {
                                Name = y.Name,
                                Price = y.Price
                            }).ToList(),
                        },
                })
                .OrderByDescending(x => x.SoldProducts.Count)
                .ToList();

            var toJson = new
            {
                UsersCount = users.Count(),
                Users = users,
            };

            var jsonText = JsonConvert.SerializeObject(toJson, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,

            });

            return jsonText;
        }
    }
}