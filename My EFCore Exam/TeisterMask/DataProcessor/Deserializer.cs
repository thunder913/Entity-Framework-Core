namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ProjectimportDto[]), new XmlRootAttribute("Projects"));
            var projectsDto = (ProjectimportDto[])serializer.Deserialize(new StringReader(xmlString));

            foreach (var proj in projectsDto)
            {
                DateTime? dueDate = new DateTime();
                if (string.IsNullOrWhiteSpace(proj.DueDate))
                {
                    dueDate = null;
                }
                else
                {
                    dueDate = DateTime.ParseExact(proj.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

                if (string.IsNullOrWhiteSpace(proj.OpenDate))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var project = new Project()
                {
                    Name = proj.Name,
                    DueDate = dueDate,
                    OpenDate = DateTime.ParseExact(proj.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                };

                if (!IsValid(project))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                foreach (var item in proj.Tasks)
                {
                    if (!IsValid(item) || !Enum.IsDefined(typeof(ExecutionType), item.ExecutionType) && !Enum.IsDefined(typeof(LabelType), item.LabelType))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var task = new Task()
                    {
                        DueDate = DateTime.ParseExact(item.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Name = item.Name,
                        OpenDate = DateTime.ParseExact(item.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Project = project,
                    };

                    if (task.OpenDate < project.OpenDate || task.DueDate > project.DueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    task.LabelType = task.LabelType;
                    task.ExecutionType = task.ExecutionType;

                    project.Tasks.Add(task);
                }

                context.Projects.Add(project);
                sb.AppendLine(string.Format(SuccessfullyImportedProject, project.Name, project.Tasks.Count));
            }

            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var employees = JsonConvert.DeserializeObject<EmployeeImportDto[]>(jsonString);
            var sb = new StringBuilder();

            foreach (var emp in employees)
            {
                if (!IsValid(emp))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var employee = new Employee()
                {
                    Email = emp.Email,
                    Phone = emp.Phone,
                    Username = emp.Username
                };

                var tasks = new HashSet<int>();
                foreach (var item in emp.Tasks)
                {
                    tasks.Add(item);
                }

                foreach (var item in tasks)
                {
                    if (!context.Tasks.Any(x => x.Id == item))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    employee.EmployeesTasks.Add(new EmployeeTask()
                    {
                        Employee = employee,
                        TaskId = item
                    });
                }

                sb.AppendLine(string.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count));
                context.Employees.Add(employee);
            }

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}