namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            bool hasError = false;
            var elements = JsonConvert.DeserializeObject<DepartmentDto[]>(jsonString);
            foreach (var depart in elements)
            {
                if (depart.Cells.Count()>0)
                {
                    var cells = new List<Cell>();
                    for (int i = 0; i < depart.Cells.Count(); i++)
                    {
                        var cell = depart.Cells[i];
                        if (cell.CellNumber>=1&&cell.CellNumber<=1000)
                        {
                            cells.Add(new Cell() 
                            {
                                CellNumber=cell.CellNumber,
                                HasWindow=cell.HasWindow
                            });
                        }
                        else
                        {
                            hasError = true;
                            break;
                        }
                    }

                    if (!hasError)
                    {
                        var department = new Department()
                        {
                            Name = depart.Name,
                            Cells = cells
                        };

                        if (IsValid(department))
                        {
                            context.Cells.AddRange(cells);
                            context.Departments.Add(department);
                            sb.AppendLine($"Imported {department.Name} with {department.Cells.Count()} cells");
                        }
                        else
                        {
                            hasError = true;
                        }

                    }
                }
                if (hasError)
                {
                    sb.AppendLine(ErrorMessage);
                    hasError = !hasError;
                }

            }

            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var culture = CultureInfo.InvariantCulture;
            var prisonersDto = JsonConvert.DeserializeObject<PrisonerDto[]>(jsonString, new JsonSerializerSettings()
            {
                Culture=culture
            });
            var sb = new StringBuilder();
            var hasError = false;
            foreach (var pris in prisonersDto)
            {
                if (IsValid(pris))
                {
                    foreach (var mail in pris.Mails)
                    {
                        if (!IsValid(mail))
                        {
                            hasError = true;
                        }
                    }

                    if (!hasError)
                    {
                        DateTime? releaseDate=null;
                        if (pris.ReleaseDate!=null)
                        {
                            releaseDate = DateTime.ParseExact(pris.ReleaseDate, "dd/MM/yyyy", culture);
                        }
                        var prisonerToAdd = new Prisoner()
                        {
                            FullName = pris.FullName,
                            Nickname = pris.Nickname,
                            Age = pris.Age,
                            ReleaseDate= releaseDate,
                            IncarcerationDate = DateTime.ParseExact(pris.IncarcerationDate, "dd/MM/yyyy", culture),
                            Bail = pris.Bail,
                            CellId = pris.CellId,
                            Mails = pris.Mails.Select(x => new Mail()
                            {
                                Address = x.Address,
                                Description = x.Description,
                                Sender = x.Sender
                            })
                            .ToArray()
                        };

                        context.Mails.AddRange(prisonerToAdd.Mails);
                        context.Prisoners.Add(prisonerToAdd);
                        sb.AppendLine($"Imported {prisonerToAdd.FullName} {prisonerToAdd.Age} years old");

                    }
                }
                else
                {
                    hasError = true;
                }

                if (hasError)
                {
                    sb.AppendLine(ErrorMessage);
                    hasError = false;
                }
            }
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(OfficerDto[]), new XmlRootAttribute("Officers"));
            var officers = (OfficerDto[])serializer.Deserialize(new StringReader(xmlString));
            var hasError = false;
            var sb = new StringBuilder();
            foreach (var officer in officers)
            {
                if (IsValid(officer) && Enum.IsDefined(typeof(Position), officer.Position) && Enum.IsDefined(typeof(Weapon), officer.Weapon))
                {
                    var officerToAdd = new Officer()
                    { 
                        FullName=officer.FullName,
                        Salary=officer.Salary,
                        Position= (Position) Enum.Parse(typeof(Position), officer.Position),
                        Weapon = (Weapon) Enum.Parse(typeof(Weapon), officer.Weapon),
                        DepartmentId=officer.DepartmentId,
                    };

                    context.Officers.Add(officerToAdd);
                    foreach (var prisoner in officer.Prisoners)
                    {
                        var officerPrisoner = new OfficerPrisoner()
                        {
                            Officer = officerToAdd,
                            PrisonerId = prisoner.Id
                        };
                        context.OfficersPrisoners.Add(officerPrisoner);
                    }

                    sb.AppendLine($"Imported {officer.FullName} ({officer.Prisoners.Count()} prisoners)");
                }
                else
                {
                    hasError = true;
                }

                if (hasError)
                {
                    hasError = false;
                    sb.AppendLine(ErrorMessage);
                }

            }
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}