using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();
            var inputJson = File.ReadAllText("Datasets/suppliers.json");
            //Console.WriteLine(ImportSuppliers(context, inputJson));
            //inputJson = File.ReadAllText("Datasets/parts.json");
            //Console.WriteLine(ImportParts(context, inputJson));
            //inputJson = File.ReadAllText("Datasets/cars.json");
            //Console.WriteLine(ImportCars(context, inputJson));
            //inputJson = File.ReadAllText("Datasets/customers.json");
            //Console.WriteLine(ImportCustomers(context, inputJson));
            //inputJson = File.ReadAllText("Datasets/sales.json");
            //Console.WriteLine(ImportSales(context, inputJson));
            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);
            context.AddRange(suppliers);
            context.SaveChanges();
            return $"Successfully imported {suppliers.Count()}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<List<Part>>(inputJson);
            foreach (var item in parts)
            {
                if (context.Suppliers.Find(item.SupplierId) != null)
                {
                    context.Add(item);
                }
            }
            var changes = context.SaveChanges();
            return $"Successfully imported {changes}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            //Commented in order for judge to work
            //Mapper.Initialize(cfg => cfg.AddProfile(new CarDealerProfile()));
            var cars = JsonConvert.DeserializeObject<List<CarImportDto>>(inputJson);

            foreach (var car in cars)
            {
                var actualCar = Mapper.Map<Car>(car);
                context.Cars.Add(actualCar);
                foreach (var partId in car.PartsId)
                {
                    context.PartCars.Add(new PartCar()
                    {
                        Car = actualCar,
                        PartId = partId
                    });

                }
            }
            var changes = context.SaveChanges();
            return $"Successfully imported {cars.Count()}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);
            context.Customers.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Count()}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);
            context.Sales.AddRange(sales);
            context.SaveChanges();
            return $"Successfully imported {sales.Count()}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers =
                context
                .Customers
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .Select(x=>new
                {
                    x.Name,
                    BirthDate=x.BirthDate.ToString("dd/MM/yyyy"),
                    x.IsYoungDriver
                })
                .ToList();

            var jsonText = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return jsonText;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars =
                context
                .Cars
                .Where(x => x.Make == "Toyota")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(x => new
                {
                    x.Id,
                    x.Make,
                    x.Model,
                    x.TravelledDistance
                })
                .ToList();

            var jsonText = JsonConvert.SerializeObject(cars, Formatting.Indented);
            return jsonText;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers =
                context
                .Suppliers
                .Where(x => !x.IsImporter)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    PartsCount = x.Parts.Count()
                })
                .ToList();

            var jsonText = JsonConvert.SerializeObject(suppliers, Formatting.Indented);
            return jsonText;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars =
                context
                .Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TravelledDistance
                    },
                    parts = c.PartCars.Select(p => new
                    {
                        Name = p.Part.Name,
                        Price = $"{p.Part.Price:f2}"
                    })
                })
                .ToList();

            var jsonText = JsonConvert.SerializeObject(cars, Formatting.Indented);
            return jsonText;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers =
                context
                .Customers
                .Where(x => x.Sales.Any(y => y.Customer.Id == x.Id))
                .Select(x => new
                {
                    fullName = x.Name,
                    boughtCars = x.Sales.Count(),
                    spentMoney = x.Sales.Sum(y=>y.Car.PartCars.Sum(p=>p.Part.Price))
                })
                .OrderByDescending(x => x.spentMoney)
                .ThenByDescending(x => x.boughtCars)
                .ToList();

            var jsonText = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return jsonText;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales =
                context
                .Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        s.Car.Make,
                        s.Car.Model,
                        s.Car.TravelledDistance
                    },
                    customerName = s.Customer.Name,
                    Discount = $"{s.Discount:f2}",
                    price = $"{s.Car.PartCars.Sum(pc => pc.Part.Price):f2}",
                    priceWithDiscount = $"{s.Car.PartCars.Sum(pc => pc.Part.Price) * (1 - s.Discount / 100):f2}"
                })
                .ToList();

            var jsonText = JsonConvert.SerializeObject(sales, Formatting.Indented);
            return jsonText;
        }
    }
}