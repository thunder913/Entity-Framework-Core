using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile(new CarDealerProfile()));

            var dbcontext = new CarDealerContext();

            //dbcontext.Database.EnsureDeleted();
            //dbcontext.Database.EnsureCreated();
            //var text = File.ReadAllText(@"Datasets\suppliers.json");
            //Console.WriteLine(ImportSuppliers(dbcontext, text));

            //var text2 = File.ReadAllText(@"Datasets\parts.json");
            //Console.WriteLine(ImportParts(dbcontext, text2));

            //var text3 = File.ReadAllText(@"Datasets\cars.json");
            //Console.WriteLine(ImportCars(dbcontext, text3));

            //var text4 = File.ReadAllText(@"Datasets\customers.json");
            //Console.WriteLine(ImportCustomers(dbcontext, text4));

            //var text5 = File.ReadAllText(@"Datasets\sales.json");
            //Console.WriteLine(ImportSales(dbcontext, text5));

            //Console.WriteLine(GetOrderedCustomers(dbcontext));

            //Console.WriteLine(GetCarsFromMakeToyota(dbcontext));

            //Console.WriteLine(GetLocalSuppliers(dbcontext));
            Console.WriteLine(GetSalesWithAppliedDiscount(dbcontext));




        }


        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var data = JsonConvert.DeserializeObject<Supplier[]>(inputJson);
            context.AddRange(data);
            context.SaveChanges();
            return $"Successfully imported {data.Count()}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            List<Part> data = JsonConvert.DeserializeObject<Part[]>(inputJson).ToList();
            data = data.Where(x => context.Suppliers.Any(c => c.Id == x.SupplierId)).ToList();
            context.Parts.AddRange(data);
            context.SaveChanges();
            return $"Successfully imported {data.Count()}.";
        }
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var data = JsonConvert.DeserializeObject<CarDTO[]>(inputJson);
            var cars = Mapper.Map<Car[]>(data);
            List<PartCarsDTO> PartCar = Mapper.Map<PartCarsDTO[]>(data).ToList();
            List<PartCar> partCars = new List<PartCar>();
            int carId = 1;
            foreach (var item in PartCar)
            {
                foreach (var part in item.PartId)
                {
                    var itemToAdd = new PartCar { CarId = carId, PartId = part };
                    if (partCars.Any(x => x.CarId == carId && x.PartId == part))
                    {
                    }
                    else
                    {
                        partCars.Add(itemToAdd);
                    }
                }
                carId++;
            }

            context.PartCars.AddRange(partCars);
            context.Cars.AddRange(cars);
            context.SaveChanges();
            return $"Successfully imported {data.Count()}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var data = JsonConvert.DeserializeObject<Customer[]>(inputJson);
            context.AddRange(data);
            context.SaveChanges();
            return $"Successfully imported {data.Count()}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var data = JsonConvert.DeserializeObject<Sale[]>(inputJson);
            context.AddRange(data);
            context.SaveChanges();
            return $"Successfully imported {data.Count()}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .Select(x => new { x.Name, BirthDate = x.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), x.IsYoungDriver })
                .ToList();

            var jsonText = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return jsonText;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var toyotaCars = context.Cars
                .Where(x => x.Make == "Toyota")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(x => new { x.Id, x.Make, x.Model, x.TravelledDistance })
                .ToList();

            var jsonText = JsonConvert.SerializeObject(toyotaCars, Formatting.Indented);

            return jsonText;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new { x.Id, x.Name, PartsCount = x.Parts.Count() })
                .ToList();

            var jsonText = JsonConvert.SerializeObject(suppliers, Formatting.Indented);
            return jsonText;
        }

        //50/100
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Include(x => x.PartCars)
                .ThenInclude(x => x.Part)
                .ToList();

            var output = Mapper.Map<List<CarPartExportTDO>>(cars);


            var jsonText = JsonConvert.SerializeObject(output,
                new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented });
            return jsonText;
            //100/100, There is literally no difference!
            //var cars = context.Cars
            //    .Include(c => c.PartCars)
            //    .ThenInclude(c => c.Part)
            //    .Select(c => new
            //    {
            //        car = new
            //        {
            //            Make = c.Make,
            //            Model = c.Model,
            //            TravelledDistance = c.TravelledDistance
            //        },

            //        parts = c.PartCars
            //        .Select(p => new
            //        {
            //            Name = p.Part.Name,
            //            Price = $"{p.Part.Price:F2}"
            //        })
            //        .ToList()
            //    })
            //    .ToList();

            //var json = JsonConvert.SerializeObject(cars, new JsonSerializerSettings()
            //{
            //    NullValueHandling = NullValueHandling.Ignore,
            //    Formatting = Formatting.Indented
            //});

            //return json;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Include(x => x.Sales)
                .Where(x => x.Sales.Any())
                .Select(x => new { fullName = x.Name, boughtCars = x.Sales.Count(), spentMoney = x.Sales.Sum(y => y.Car.PartCars.Sum(z => z.Part.Price)) })
                .OrderByDescending(x => x.spentMoney)
                .ThenByDescending(x => x.boughtCars)
                .ToList();

            var jsonText = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return jsonText;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var saleInformatinon = context.Sales
                .Include(x => x.Car)
                .Include(x => x.Customer)
                .OrderBy(x=>x.Id)
                .Take(10)
                .Select(x => new
                {
                    car = new
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    customerName = x.Customer.Name,
                    Discount = $"{x.Discount:f2}",
                    price = $"{x.Car.PartCars.Sum(y => y.Part.Price):f2}",
                    priceWithDiscount = $"{x.Car.PartCars.Sum(y => y.Part.Price) * (1- x.Discount/100):f2}"

                })
                .ToList();

            var jsonText = JsonConvert.SerializeObject(saleInformatinon, Formatting.Indented);

            return jsonText;
        }
    }
}