using AutoMapper;
using ProductShop.Dto;
using ProductShop.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<Product, ProductDto>();

            this.CreateMap<User, SoldProductsWithCount>()
                .ForMember(x => x.Products, y => y.MapFrom(x => x.ProductsSold.Where(z => z.Buyer != null)));

            this.CreateMap<User, UserDto>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(x => x));

            this.CreateMap<List<UserDto>, UserCountWithUsersDto>()
                .ForMember(x=>x.Users, y=>y.MapFrom(x=>x));

        }
    }
}
