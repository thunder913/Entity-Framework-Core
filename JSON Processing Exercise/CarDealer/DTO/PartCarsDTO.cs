using AutoMapper.Configuration.Conventions;
using System.Collections;
using System.Collections.Generic;

namespace CarDealer.DTO
{
    public class PartCarsDTO
    {
        public PartCarsDTO() 
        {
            this.PartId = new HashSet<int>();
        }
        public int CarId { get; set; }

        public ICollection<int> PartId { get; set; }
    }
}
