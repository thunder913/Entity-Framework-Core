using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.Export;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            //Mapper.Initialize(cfg => cfg.AddProfile(new ProductShopProfile()));
            var context = new ProductShopContext();
            //Delete and create the database
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //IMPORT THE DATA 1-4
            //var userInput = File.ReadAllText(@"C:\Users\andon\source\repos\JSON Processing Exercise\ProductShop\Datasets\users.json");
            //Console.WriteLine(ImportUsers(context, userInput));
            //var categoryInput = File.ReadAllText(@"C:\Users\andon\source\repos\JSON Processing Exercise\ProductShop\Datasets\categories.json");
            //Console.WriteLine(ImportCategories(context, categoryInput));
            //var productInput = File.ReadAllText(@"C:\Users\andon\source\repos\JSON Processing Exercise\ProductShop\Datasets\products.json");
            //Console.WriteLine(ImportProducts(context, productInput));
            //var categoryProductInput = File.ReadAllText(@"C:\Users\andon\source\repos\JSON Processing Exercise\ProductShop\Datasets\categories-products.json");
            //Console.WriteLine(ImportCategoryProducts(context, categoryProductInput));

            //Console.WriteLine(GetProductsInRange(context));
            //File.WriteAllText("categoriesByProductsMine.json", GetCategoriesByProductsCount(context));
            Console.WriteLine(GetUsersWithProducts(context));

        }
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<User[]>(inputJson);
            //Also works context.AddRange(...);
            foreach (var user in users)
            {
                context.Users.Add(user);
            }
            context.SaveChanges();
            return $"Successfully imported {users.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson);
            //Also works context.AddRange(...);
            foreach (var item in products)
            {
                context.Products.Add(item);
            }
            context.SaveChanges();
            return $"Successfully imported {products.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert
                .DeserializeObject<Category[]>(inputJson, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                .Where(x => x.Name != null);
            //Also works context.AddRange(...);

            foreach (var item in categories)
            {
                context.Categories.Add(item);
            }
            context.SaveChanges();
            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var productCategory = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);
            foreach (var item in productCategory)
            {
                context.CategoryProducts.Add(item);
            }
            context.SaveChanges();
            return $"Successfully imported {productCategory.Count()}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            List<ProductDTO> productsInRange = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .ProjectTo<ProductDTO>()
                .ToList();

            var json = JsonConvert.SerializeObject(productsInRange, Formatting.Indented);

            //File.WriteAllText("products-in-range.json", json);

            return json;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            List<UserSoldProductsDTO> users = context.Users
                .Where(x => x.ProductsSold.Any(y => y.Buyer != null))
                .ProjectTo<UserSoldProductsDTO>()
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToList();

            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            return json;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            List<CategoryDTO> categories = context.Categories
                .ProjectTo<CategoryDTO>()
                .OrderByDescending(x => x.ProductsCount)
                .ToList();
            var json = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return json;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            Mapper.Initialize(cfg => cfg.AddProfile(new ProductShopProfile()));

            List<UserDTO> users = context.Users
                .Where(x => x.ProductsSold.Any(y=>y.Buyer != null))
                .OrderByDescending(x => x.ProductsSold.Count(y=>y.Buyer != null))
                .ProjectTo<UserDTO>()
                .ToList();

            var objectToSerialize = Mapper.Map<UserProductDTO>(users);

            var json = JsonConvert.SerializeObject(objectToSerialize,
                new JsonSerializerSettings()
                {
                    ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new CamelCaseNamingStrategy(),
                    },

                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,
                });

            return json;
        }
    }
}