using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using CarDealer.DTO;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            //IMPORT PARTCAR

            this.CreateMap<CarDTO, Car>();

            this.CreateMap<CarDTO, PartCarsDTO>()
                .ForMember(x => x.CarId, y => y.MapFrom(x => x.Id))
                .ForMember(x=>x.PartId, y=>y.MapFrom(x=>x.PartsId));

            this.CreateMap<CarDTO, PartCar>()
                .ForMember(x => x.CarId, y => y.MapFrom(x => x.Id))
                .ForMember(x => x.PartId, y => y.MapFrom(x => x.PartsId));

            //CarExport
            this.CreateMap<Part, PartExportTDO>()
                .ForMember(x => x.Price, y => y.MapFrom(x => $"{x.Price:f2}"));
                
            this.CreateMap<Car, CarExportTDO>();


            this.CreateMap<Car, CarPartExportTDO>()
                .ForMember(x => x.car, y => y.MapFrom(x => x))
                .ForMember(x => x.parts, y => y.MapFrom(x => x.PartCars.Select(z=>z.Part)));
        }
    }
}
