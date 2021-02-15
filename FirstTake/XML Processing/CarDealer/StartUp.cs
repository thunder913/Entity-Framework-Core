using CarDealer.Data;
using CarDealer.DTOModels;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
            //    context.Database.EnsureDeleted();
            //    context.Database.EnsureCreated();
            //    var xmlText = File.ReadAllText(@"Datasets\suppliers.xml");
            //    System.Console.WriteLine(ImportSuppliers(context,xmlText));
            //   
            //    var xmlText1 = File.ReadAllText(@"Datasets\parts.xml");
            //    System.Console.WriteLine(ImportParts(context, xmlText1));
            //   
            //   var xmlText2 = File.ReadAllText(@"Datasets\customers.xml");
            //   System.Console.WriteLine(ImportCustomers(context, xmlText2));
            //
            //  var xmlText3 = File.ReadAllText(@"Datasets\cars.xml");
            //  System.Console.WriteLine(ImportCars(context, xmlText3));
            //
            //var xmlText4 = File.ReadAllText(@"Datasets\sales.xml");
            //System.Console.WriteLine(ImportSales(context, xmlText4));

            //System.Console.WriteLine(GetCarsWithDistance(context));

            //System.Console.WriteLine(GetCarsFromMakeBmw(context));

            //System.Console.WriteLine(GetLocalSuppliers(context));

            //System.Console.WriteLine(GetCarsWithTheirListOfParts(context));

            //System.Console.WriteLine(GetTotalSalesByCustomer(context));

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
            var parts = (Part[])serializer.Deserialize(new StringReader(inputXml));
            parts = parts
                .Where(x => context.Suppliers.Any(y => y.Id == x.SupplierId)).ToArray();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}";
        }


        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(Customer[]), new XmlRootAttribute("Customers"));
            var customers = (Customer[])serializer.Deserialize(new StringReader(inputXml));

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(Cars));
            var cars = (Cars)serializer.Deserialize(new StringReader(inputXml));

            for (int i = 0; i < cars.Car.Count(); i++)
            {
                cars.Car[i].parts.Select(x => x.id).Distinct();
            }
            var carsToImport = new List<Car>();
            var partCars = new List<PartCar>();

            int index = 0;
            foreach (var item in cars.Car)
            {
                index++;
                carsToImport.Add(new Car()
                {
                    Make = item.make,
                    Model = item.model,
                    TravelledDistance = item.TraveledDistance
                });

                foreach (var part in item.parts)
                {
                    var partCarTemp = new PartCar()
                    {
                        PartId = part.id,
                        CarId = index
                    };
                    if (!partCars.Any(x=>x.CarId == index && x.PartId == part.id))
                    {
                         partCars.Add(partCarTemp);
                    }
                }
            }
            context.Cars.AddRange(carsToImport);
            context.SaveChanges();
            partCars = partCars.Where(x => context.Cars.Any(y => y.Id == x.CarId) && context.Parts.Any(y => y.Id == x.PartId)).ToList();
            context.PartCars.AddRange(partCars);
            context.SaveChanges();
            return $"Successfully imported {carsToImport.Count()}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml) 
        {
            var serializer = new XmlSerializer(typeof(Sale[]), new XmlRootAttribute("Sales"));
            var sales = (Sale[])serializer.Deserialize(new StringReader(inputXml));

            sales = sales
                .Where(x => context.Cars.Any(y => y.Id == x.CarId))
                .ToArray();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}";
        }

        public static string GetCarsWithDistance(CarDealerContext context) 
        {
            var cars = context.Cars
                .Where(x => x.TravelledDistance > 2000000)
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .Select(x=>new CarExportDTO() 
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .ToList();

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(List<CarExportDTO>), new XmlRootAttribute("cars"));
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), cars, xmlNamespaces);
            return sb.ToString().TrimEnd();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context) 
        {
            var cars = context.Cars
                .Where(x => x.Make == "BMW")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(x=>new BMWCarExport() 
                {
                id = x.Id,
                Model = x.Model,
                TravelledDistance = x.TravelledDistance
                })
                .ToList();

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(List<BMWCarExport>), new XmlRootAttribute("cars"));
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), cars, xmlNamespaces);
            return sb.ToString().TrimEnd();
        }

        public static string GetLocalSuppliers(CarDealerContext context) 
        {
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new LocalSuppliersDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count()
                })
                .ToList();

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(List<LocalSuppliersDTO>), new XmlRootAttribute("suppliers"));
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), suppliers, xmlNamespaces);
            return sb.ToString().TrimEnd();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context) 
        {
            var cars = context.Cars
                .Include(x => x.PartCars)
                .OrderByDescending(x => x.TravelledDistance)
                .ThenBy(x => x.Model)
                .Select(x => new CarPartsDTO.carsCar()
                {
                    make = x.Make,
                    model = x.Model,
                    travelleddistance = x.TravelledDistance,
                    parts = x.PartCars.Select(y => new CarPartsDTO.carsCarPart
                    {
                        name = y.Part.Name,
                        price = y.Part.Price
                    }).OrderByDescending(x=>x.price).ToArray()
                })
                .Take(5)
                .ToList();

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(List<CarPartsDTO.carsCar>), new XmlRootAttribute("cars"));
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), cars, xmlNamespaces);
            return sb.ToString().TrimEnd();
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context) 
        {
            var customers = context.Customers
                .Where(x => x.Sales.Any(y => y.CustomerId == x.Id))
                .Select(x => new CustomerSaleDTO()
                {
                    Name = x.Name,
                    BoughtCarsCount = x.Sales.Count(),
                    MoneySpent = context.PartCars.Where(z => z.Car.Sales.Any(c => c.CustomerId == x.Id)).Sum(p => p.Part.Price)
                })
                .OrderByDescending(x=>x.MoneySpent)
                .ToList();

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(List<CustomerSaleDTO>), new XmlRootAttribute("customers"));
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), customers, xmlNamespaces);
            return sb.ToString().TrimEnd();
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context) 
        {
            var salesInfo = context.Sales
                .Select(x => new SaleDTO()
                {
                    Car = new SaleCarExportDTO()
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    CustomerName = x.Customer.Name,
                    Discount = x.Discount,
                    Price = x.Car.PartCars.Sum(y => y.Part.Price),
                    PriceAfterDiscount = (x.Car.PartCars.Sum(y => y.Part.Price) * (1 - x.Discount/100)).ToString("G29")
                })
                .ToList();

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(List<SaleDTO>), new XmlRootAttribute("sales"));
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), salesInfo, xmlNamespaces);
            return sb.ToString().TrimEnd();
        }
    }
}