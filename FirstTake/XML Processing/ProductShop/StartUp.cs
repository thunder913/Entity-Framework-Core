using ProductShop.Data;
using ProductShop.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System;
using Microsoft.EntityFrameworkCore;
using System.Text;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ProductShop.Dtos.Export;
using System.Xml;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            // context.Database.EnsureDeleted();
            // context.Database.EnsureCreated();
            // Console.WriteLine(ImportUsers(context, @"Datasets\users.xml"));
            // Console.WriteLine(ImportProducts(context, @"Datasets\products.xml"));
            // var xml = File.ReadAllText(@"Datasets\categories-products.xml");
            // Console.WriteLine(ImportCategoryProducts(context,xml));
            Console.WriteLine(GetProductsInRange(context));
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {

            var serializer = new XmlSerializer(typeof(User[]), new XmlRootAttribute("Users"));
            var users = (User[])serializer.Deserialize(File.OpenRead(inputXml));
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
            var categories = (Category[])serializer.Deserialize(new StringReader(inputXml));

            categories = categories.Where(x => x.Name != null).ToArray();

            context.Categories.AddRange(categories);
            context.SaveChanges();
            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<CategoryProduct>), new XmlRootAttribute("CategoryProducts"));
            var categoryProducts = (List<CategoryProduct>)serializer.Deserialize(new StringReader(inputXml));

            categoryProducts = categoryProducts
                .Where
                (x => context.Categories.ToList().Any(y => y.Id == x.CategoryId)
                && context.Products.ToList().Any(y => y.Id == x.ProductId)).ToList();

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count()}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Include(x => x.Buyer)
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Take(10)
                .Select(x => new ProductDTO
                {
                    Name = x.Name,
                    Price = x.Price,
                    BuyerName = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .ToList();

            //Judge doesnt works with list 
            var serializer = new XmlSerializer(typeof(List<ProductDTO>), new XmlRootAttribute("Products"));
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), products, xmlNamespaces);
            return sb.ToString();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any())
                .Select(x => new UserSoldProductsDTO
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(y => new SoldProduct
                    {
                        Name = y.Name,
                        Price = y.Price
                    }).ToList()
                })
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .ToList();

            var serializer = new XmlSerializer(typeof(List<UserSoldProductsDTO>), new XmlRootAttribute("Users"));
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), users, xmlNamespaces);
            return sb.ToString().TrimEnd();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(x => new CategoryDTO
                {
                    Name = x.Name,
                    ProductsCount = x.CategoryProducts.Count(y => y.CategoryId == x.Id),
                    AveragePrice = Math.Round(x.CategoryProducts.Average(y => y.Product.Price), 28),
                    TotalRevenue = x.CategoryProducts.Sum(y => y.Product.Price)
                })
                .OrderByDescending(x => x.ProductsCount)
                .ThenBy(x => x.TotalRevenue)
                .ToList();

            var serializer = new XmlSerializer(typeof(List<CategoryDTO>), new XmlRootAttribute("Categories"));
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), categories, xmlNamespaces);
            return sb.ToString().TrimEnd();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            Mapper.Initialize(cfg => cfg.AddProfile(new ProductShopProfile()));

            var users = context.Users  
                .Where(u => u.ProductsSold.Count > 0)
                .ToArray()
                .ProjectTo<UserDTO>()
                .ToList()
                .OrderByDescending(u => u.SoldProducts.Count)
                .ToList();

            for (int i = 0; i < users.Count(); i++)
            {
                users[i].SoldProducts.Products = users[i].SoldProducts.Products.OrderByDescending(x => x.Price).ToList();
            }

            var userToSerialize = Mapper.Map<UserProductDTO>(users);
            
            userToSerialize.Users = userToSerialize.Users.Take(10).ToList();
            
            var serializer = new XmlSerializer(typeof(UserProductDTO), new XmlRootAttribute("Users"));
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();
            
            serializer.Serialize(new StringWriter(sb), userToSerialize, xmlNamespaces);
            return sb.ToString().TrimEnd();
        }
    }
}