using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();
            //var supplierXml = File.ReadAllText("Datasets/suppliers.xml");
            //System.Console.WriteLine(ImportSuppliers(context, supplierXml));
            //var partXml = File.ReadAllText("Datasets/parts.xml");
            //System.Console.WriteLine(ImportParts(context, partXml));
            //var carXml = File.ReadAllText("Datasets/cars.xml");
            //System.Console.WriteLine(ImportCars(context, carXml));
            //var customerXml = File.ReadAllText("Datasets/customers.xml");
            //System.Console.WriteLine(ImportCustomers(context, customerXml));
            //var salesXml = File.ReadAllText("Datasets/sales.xml");
            //System.Console.WriteLine(ImportSales(context, salesXml));
            System.Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(Supplier[]), new XmlRootAttribute("Suppliers"));
            var suppliers = (Supplier[])serializer.Deserialize(new StringReader(inputXml));
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();
            return $"Successfully imported {suppliers.Count()}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(Part[]), new XmlRootAttribute("Parts"));
            var parts = ((Part[])serializer.Deserialize(new StringReader(inputXml))).Where(x=>context.Suppliers.Any(y=>y.Id==x.SupplierId));
            context.Parts.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {parts.Count()}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(Cars), new XmlRootAttribute("Cars"));
            var cars = ((Cars)serializer.Deserialize(new StringReader(inputXml))).Car;

            foreach (var car in cars)
            {
                var actualCar = new Car()
                {
                    Make = car.make,
                    Model = car.model,
                    TravelledDistance = car.TraveledDistance,
                };
                var parts = new HashSet<int>();
                foreach (var part in car.parts)
                {
                    if (!parts.Contains(part.id))
                    {
                        var partCar = new PartCar()
                        {
                            Car = actualCar,
                            PartId = part.id
                        };
                        context.PartCars.Add(partCar);
                        actualCar.PartCars.Add(partCar);
                    }
                    parts.Add(part.id);
                }

                context.Cars.Add(actualCar);
            }
            context.SaveChanges();

            return $"Successfully imported {cars.Count()}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(Customer[]), new XmlRootAttribute("Customers"));
            var customers = (Customer[]) serializer.Deserialize(new StringReader(inputXml));
            context.Customers.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Count()}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(Sale[]), new XmlRootAttribute("Sales"));
            var sales = (Sale[])serializer.Deserialize(new StringReader(inputXml));
            sales = sales.Where(x => context.Cars.Find(x.CarId) != null).ToArray();
            context.Sales.AddRange(sales);
            context.SaveChanges();
            return $"Successfully imported {sales.Count()}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars =
                context
                .Cars
                .Where(x => x.TravelledDistance > 2000000)
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .Select(x => new CarExportDto()
                {
                    Make=x.Make,
                    Model=x.Model,
                    TravelledDistance=x.TravelledDistance
                })

                .ToArray();

            var serializer = new XmlSerializer(typeof(CarExportDto[]), new XmlRootAttribute("cars"));
            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), cars, new XmlSerializerNamespaces(new[] { new XmlQualifiedName("","") }));
            return sb.ToString().TrimEnd();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars =
                context
                .Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new CarMakeDto()
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToArray();

            var serializer = new XmlSerializer(typeof(CarMakeDto[]), new XmlRootAttribute("cars"));
            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), cars, new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") }));
            return sb.ToString().TrimEnd();
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers =
                context
                .Suppliers
                .Where(s => !s.IsImporter)
                .Select(s => new SupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count()
                })
                .ToArray();

            var serializer = new XmlSerializer(typeof(SupplierDto[]), new XmlRootAttribute("suppliers"));
            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), suppliers, new XmlSerializerNamespaces(new[] { new XmlQualifiedName("","") }));
            return sb.ToString().TrimEnd();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars =
                context
                .Cars
                .OrderByDescending(x=>x.TravelledDistance)
                .ThenBy(x=>x.Model)
                .Take(5)
                .Select(x => new CarPartsDto()
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,
                    Parts = x.PartCars.Select(y => new PartsDto()
                    {
                        Name = y.Part.Name,
                        Price = y.Part.Price
                    })
                    .OrderByDescending(y => y.Price)
                    .ToArray()
                })
                .ToArray();

            var serializer = new XmlSerializer(typeof(CarPartsDto[]), new XmlRootAttribute("cars"));
            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), cars, new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") }));
            return sb.ToString().TrimEnd();
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers =
                context
                .Customers
                .Where(x => x.Sales.Any(y => y.Car != null))
                .Select(x => new CustomerSaleDto()
                {
                    FullName = x.Name,
                    BoughtCars = x.Sales.Count(y => y.Car != null),
                    SpentMoney= context.PartCars.Where(z=>z.Car.Sales.Any(s=>s.CustomerId==x.Id)).Sum(p=>p.Part.Price)
                })
                .OrderByDescending(x=>x.SpentMoney)
                .ToArray();


            var serializer = new XmlSerializer(typeof(CustomerSaleDto[]), new XmlRootAttribute("customers"));
            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), customers, new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") }));
            return sb.ToString().TrimEnd();
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales =
                context
                .Sales
                .Select(s => new SaleCarCustomerDto()
                {
                    CustomerName = s.Customer.Name,
                    Discount = s.Discount,
                    Price = s.Car.PartCars.Sum(pc => pc.Part.Price),
                    PriceWthDicount = (s.Car.PartCars.Sum(pc => pc.Part.Price) * (1 - s.Discount/100)).ToString("G29"),
                    CarInfoDto = new CarInfoDto()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance =  s.Car.TravelledDistance
                    }
                })
                .ToArray();

            var serializer = new XmlSerializer(typeof(SaleCarCustomerDto[]), new XmlRootAttribute("sales"));
            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), sales, new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") }));
            return sb.ToString().TrimEnd();
        }
    }
}