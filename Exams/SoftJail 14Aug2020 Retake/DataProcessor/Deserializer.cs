namespace SoftJail.DataProcessor
{

    using Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ImportDto;
    using System.Linq;
    using System.Text;
    using SoftJail.Data.Models;
    using System.Text.RegularExpressions;
    using System.Globalization;

    public class Deserializer
    {
        public const string ErrorMessage = "Invalid Data";
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var departments = JsonConvert.DeserializeObject<DepartmentImportDTO[]>(jsonString);
            var sb = new StringBuilder();
            foreach (var department in departments)
            {
                if (department.Name.Length < 3 || department.Name.Length > 25
                    || department.Cells.Count() == 0 || department.Cells.Any(x => x.CellNumber < 1 || x.CellNumber > 1000))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var currentDepartment = new Department()
                {
                    Name = department.Name
                };
                foreach (var cell in department.Cells)
                {
                    var cellToAdd = new Cell()
                    {
                        CellNumber = cell.CellNumber,
                        HasWindow = cell.HasWindow,
                        Department = currentDepartment
                    };
                    context.Cells.Add(cellToAdd);
                    currentDepartment.Cells.Add(cellToAdd);
                }

                context.Departments.Add(currentDepartment);
                sb.AppendLine($"Imported {currentDepartment.Name} with {currentDepartment.Cells.Count()} cells");
            }
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var dateRegex = new Regex(@"^[0-9]{2}\/[0-9]{2}\/[0-9]{4}$");
            var addressRegex = new Regex(@"^[A-Za-z0-9 ]+str\.$");
            var nicknameRegex = new Regex(@"^The [A-Z][a-z]*$");
            var prisoners = JsonConvert.DeserializeObject<PrisonerimportDTO[]>(jsonString);
            var sb = new StringBuilder();
            foreach (var prisoner in prisoners)
            {
                if (prisoner.IncarcerationDate == null //|| prisoner.ReleaseDate == null
                    || !dateRegex.IsMatch(prisoner.IncarcerationDate) //|| !dateRegex.IsMatch(prisoner.ReleaseDate)
                   || string.IsNullOrWhiteSpace(prisoner.FullName) ||string.IsNullOrWhiteSpace(prisoner.Nickname)
                    || prisoner.FullName.Length < 3 || prisoner.FullName.Length > 20
                    || prisoner.Age < 18 || prisoner.Age > 65 || !nicknameRegex.IsMatch(prisoner.Nickname)
                    || prisoner.Mails.Any(x => !addressRegex.IsMatch(x.Address)))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var prisonerToAdd = new Prisoner()
                {
                    Age = prisoner.Age,
                    Bail = prisoner.Bail,
                    CellId = prisoner.CellId,
                    FullName = prisoner.FullName,
                    IncarcerationDate = DateTime.ParseExact(prisoner.IncarcerationDate, @"dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Nickname = prisoner.Nickname
                };

                if (prisoner.ReleaseDate == null)
                {
                    prisonerToAdd.ReleaseDate = null;
                }
                else 
                {
                    prisonerToAdd.ReleaseDate = DateTime.ParseExact(prisoner.ReleaseDate, @"dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                foreach (var mail in prisoner.Mails)
                {
                    var mailToAdd = new Mail()
                    {
                        Address = mail.Address,
                        Sender = mail.Sender,
                        Description = mail.Description,
                        Prisoner = prisonerToAdd
                    };
                    context.Mails.Add(mailToAdd);
                    prisonerToAdd.Mails.Add(mailToAdd);
                }

                context.Prisoners.Add(prisonerToAdd);
                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            throw new NotImplementedException();
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