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
            this.CreateMap<Product, ProductDto>();

            this.CreateMap<User, SoldProducts>()
                .ForMember(x => x.Products, y => y.MapFrom(x => x.ProductsSold.Where(z => z.Buyer != null)))
                .ForMember(x => x.Count, y => y.MapFrom(x => x.ProductsSold.Where(z => z.Buyer != null).Count()));

            this.CreateMap<User, UserProductDto>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(x => x.ProductsSold.Where(z=>z.Buyer!=null)));

            this.CreateMap<List<UserProductDto>, UserCountDto>()
                .ForMember(x=>x.Users, y=>y.MapFrom(x=>x));

        }
    }
}
