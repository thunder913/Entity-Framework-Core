using AutoMapper;
using ProductShop.Dtos.Export;
using ProductShop.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            //Users and Products

            this.CreateMap<Product, ProductsDTO>();

            this.CreateMap<User, SoldProducts>()
                .ForMember(x => x.Products, y => y.MapFrom(x => x.ProductsSold))
                .ForMember(x => x.Count, y => y.MapFrom(x => x.ProductsSold.Count()));

            this.CreateMap<User, UserDTO>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(x => x));

            this.CreateMap<List<UserDTO>, UserProductDTO>()
                .ForMember(x => x.Users, y => y.MapFrom(x => x))
                .ForMember(x => x.Count, y => y.MapFrom(x => x.Count()));

        }
    }
}
