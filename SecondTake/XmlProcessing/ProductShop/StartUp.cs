using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();
            //var usersXML = XDocument.Load("Datasets/users.xml").ToString();
            //System.Console.WriteLine(ImportUsers(context, usersXML));
            //var productsXml = XDocument.Load("Datasets/products.xml").ToString();
            //System.Console.WriteLine(ImportProducts(context, productsXml));
            //var categories = XDocument.Load("Datasets/categories.xml").ToString();
            //System.Console.WriteLine(ImportCategories(context, categories));
            //var categories = XDocument.Load("Datasets/categories-products.xml").ToString();
            //System.Console.WriteLine(ImportCategoryProducts(context, categories));
            //System.Console.WriteLine(GetProductsInRange(context));
            //System.Console.WriteLine(GetSoldProducts(context));
            //System.Console.WriteLine(GetCategoriesByProductsCount(context));
            System.Console.WriteLine(GetUsersWithProducts(context));
            
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(User[]), new XmlRootAttribute("Users"));
            var users = (User[])serializer.Deserialize(new StringReader(inputXml));
            context.Users.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(Product[]), new XmlRootAttribute("Products"));
            var products = (Product[])serializer.Deserialize(new StringReader(inputXml));
            context.Products.AddRange(products);
            context.SaveChanges();
            return $"Successfully imported {products.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(Category[]), new XmlRootAttribute("Categories"));
            var categories = ((Category[])serializer.Deserialize(new StringReader(inputXml))).Where(x=>x.Name!=null);
            context.Categories.AddRange(categories);
            context.SaveChanges();
            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(CategoryProduct[]), new XmlRootAttribute("CategoryProducts"));
            var categoryProducts = (CategoryProduct[])serializer.Deserialize(new StringReader(inputXml));
            foreach (var item in categoryProducts)
            {
                if (context.Categories.Find(item.CategoryId) != null && context.Products.Find(item.ProductId)!=null)
                {
                    context.Add(item);
                }
            }
            var count = context.SaveChanges();
            return $"Successfully imported {count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products =
                context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new UserDto
                {
                    Name=p.Name,
                    Price=p.Price,
                    Buyer= p.Buyer.FirstName + " " + p.Buyer.LastName
                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToArray();

            var serializer = new XmlSerializer(typeof(UserDto[]), new XmlRootAttribute("Products"));
            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), products, new XmlSerializerNamespaces(new[] { new XmlQualifiedName("","")}));
            return sb.ToString().TrimEnd();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users =
                context
                .Users
                .Where(x => x.ProductsSold.Count(y => y.Buyer != null) >= 1)
                .OrderBy(x=>x.LastName)
                .ThenBy(x=>x.FirstName)
                .Take(5)
                .Select(x => new UserSoldItemDto()
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(y => new ProductSoldDto()
                    {
                        Name = y.Name,
                        Price = y.Price
                    })
                    .ToList()
                })
                .ToArray();

            var serializer = new XmlSerializer(typeof(UserSoldItemDto[]), new XmlRootAttribute("Users"));
            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), users, new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") }));
            return sb.ToString().TrimEnd();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories =
                context
                .Categories
                .Select(x => new CategoryDto()
                {
                    Name = x.Name,
                    Count = x.CategoryProducts.Count,
                    AveragePrice = Math.Round(x.CategoryProducts.Average(y => y.Product.Price), 28),
                    TotalRevenue = x.CategoryProducts.Sum(y => y.Product.Price)
                })
                .OrderByDescending(x=>x.Count)
                .ThenBy(x=>x.TotalRevenue)
                .ToArray();

            var serializer = new XmlSerializer(typeof(CategoryDto[]), new XmlRootAttribute("Categories"));
            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), categories, new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") }));
            return sb.ToString().TrimEnd();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users =
                context
                .Users
                .Include(x => x.ProductsSold)
                .ToList()
                .Where(x => x.ProductsSold.Any(y => y.Buyer != null))
                .Select(x => new UserProductDto()
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    SoldProducts = new SoldProducts()
                    {
                        Count = x.ProductsSold.Count(z => z.Buyer != null),
                        Products = x.ProductsSold.Where(z => z.Buyer != null).Select(v => new ProductDto()
                        {
                            Name = v.Name,
                            Price = v.Price
                        })
                        .OrderByDescending(b => b.Price)
                        .ToArray()
                    }
                })
                .OrderByDescending(x => x.SoldProducts.Count)
                .Take(10)
                .ToArray();

            var usersCount = new UserCountDto()
            {
                Count = context.Users.Count(x => x.ProductsSold.Any(y => y.Buyer != null)),
                Users = users
            };

            var serializer = new XmlSerializer(typeof(UserCountDto), new XmlRootAttribute("Users"));
            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), usersCount, new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") }));
            return sb.ToString().TrimEnd();

        }
    }
}