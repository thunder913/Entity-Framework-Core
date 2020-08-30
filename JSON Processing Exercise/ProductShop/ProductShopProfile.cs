using AutoMapper;
using AutoMapper.Configuration.Conventions;
using ProductShop.Export;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<Product, ProductDTO>()
                .ForMember(x => x.SellerName, y => y.MapFrom(x => x.Seller.FirstName + " " + x.Seller.LastName));
            
            //UserSoldProducts

            this.CreateMap<User, UserSoldProductsDTO>();

            this.CreateMap<Product, Products>()
                .ForMember(x => x.BuyerFirstName, y => y.MapFrom(x => x.Buyer.FirstName))
                .ForMember(x => x.BuyerLastName, y => y.MapFrom(x => x.Buyer.LastName));

            //CategoryDTO

            this.CreateMap<Category, CategoryDTO>()
                .ForMember(x => x.ProductsCount, y => y.MapFrom(x => x.CategoryProducts.Count()))
                .ForMember(x => x.AveragePrice, y => y.MapFrom(x => Math.Round(x.CategoryProducts.Average(z => z.Product.Price),2).ToString()))
                .ForMember(x => x.Revenue, y => y.MapFrom(x => Math.Round(x.CategoryProducts.Sum(z => z.Product.Price),2).ToString()));

            //Users and Products

            this.CreateMap<Product, ProductDTO>()
                .ForMember(x => x.Price, y => y.MapFrom(x => Decimal.Round(x.Price, 1)));

            this.CreateMap<User, SoldProductsDTO>()
                .ForMember(x => x.Products, y => y.MapFrom(x => x.ProductsSold.Where(z=>z.Buyer != null)));

            this.CreateMap<User, UserDTO>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(x => x));

            this.CreateMap<List<UserDTO>, UserProductDTO>()
                .ForMember(x => x.Users, y => y.MapFrom(x => x));
        }
    }
}
