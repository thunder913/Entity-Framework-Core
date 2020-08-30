using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace ProductShop.Export
{
    public class UserProductDTO
    {
        public UserProductDTO() 
        {
            this.Users = new HashSet<UserDTO>();
        }
        public int UsersCount => this.Users.Count();

        public ICollection<UserDTO> Users { get; set; }
    }

    public class UserDTO 
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Age { get; set; }

        public SoldProductsDTO SoldProducts { get; set; } = new SoldProductsDTO();
    }

    public class SoldProductsDTO 
    {
        public SoldProductsDTO() 
        {
            this.Products = new HashSet<ProductsDTO>();
        }
        public int Count => this.Products.Count();

        public ICollection<ProductsDTO> Products { get; set; }
    }

    public class ProductsDTO 
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }


}
