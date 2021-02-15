using System;
using SoftUni.Data;
using SoftUni.Models;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();
            //Console.WriteLine(GetEmployeesFullInformation(context));
            //Console.WriteLine(GetEmployeesWithSalaryOver50000(context));
            //Console.WriteLine(GetEmployeesFromResearchAndDevelopment(context));
            //Console.WriteLine(AddNewAddressToEmployee(context));
            //Console.WriteLine(GetEmployeesInPeriod(context));
            //Console.WriteLine(GetAddressesByTown(context));
            //Console.WriteLine(GetEmployee147(context));
            //Console.WriteLine(GetDepartmentsWithMoreThan5Employees(context));
            //Console.WriteLine(GetLatestProjects(context));
            //Console.WriteLine(IncreaseSalaries(context));
            // Console.WriteLine(GetEmployeesByFirstNameStartingWithSa(context));
            //Console.WriteLine(DeleteProjectById(context));
            Console.WriteLine(RemoveTown(context));
        }

        //Problem 3
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.MiddleName,
                    x.JobTitle,
                    x.Salary,
                    x.EmployeeId
                })
                .OrderBy(x => x.EmployeeId)
                .ToList();
            var sb = new StringBuilder();
            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }
        //Problem 4
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(x => x.Salary > 50000)
                .Select(x => new { x.FirstName, x.Salary })
                 .OrderBy(x => x.FirstName)
                 .ToList();

            var sb = new StringBuilder();
            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} - {emp.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 5
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(x => x.Department.Name == "Research and Development")
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    DepartmentName = x.Department.Name,
                    x.Salary
                }).OrderBy(x => x.Salary).ThenByDescending(x => x.FirstName).ToList();

            var sb = new StringBuilder();
            foreach (var item in employees)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} from {item.DepartmentName} - ${item.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 6
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var adress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Addresses.Add(adress);

            context.Employees.First(x => x.LastName == "Nakov").Address = adress;

            context.SaveChanges();
            var addresses = context.Addresses
                .OrderByDescending(x => x.AddressId)
                .Take(10)
                .Select(x => x.AddressText)
                .ToList();

            var sb = new StringBuilder();
            foreach (var item in addresses)
            {
                sb.AppendLine(item);
            }
            return sb.ToString().TrimEnd();
        }
        //Problem 7
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(x => x.EmployeesProjects.Any(x => x.Project.StartDate.Year >= 2001 && x.Project.StartDate.Year <= 2003))
                    .Select(x => new
                    {
                        projects = x.EmployeesProjects
                                    .Select(x => new
                                    {
                                        ProjectName = x.Project.Name,
                                        ProjectStart = x.Project.StartDate,
                                        ProjectEnd = x.Project.EndDate
                                    }),
                        x.FirstName,
                        x.LastName,
                        ManagerFirstName = x.Manager.FirstName,
                        ManagerLastName = x.Manager.LastName
                    })
                .Take(10)
                .ToList();

            var sb = new StringBuilder();
            foreach (var item in employees)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} - Manager: {item.ManagerFirstName} {item.ManagerLastName}");
                foreach (var project in item.projects)
                {
                    if (project.ProjectEnd == null)
                    {
                        sb.AppendLine($"--{project.ProjectName} - {project.ProjectStart.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} - not finished");
                    }
                    else
                    {
                        sb.AppendLine($"--{project.ProjectName} - {project.ProjectStart.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} - {project.ProjectEnd?.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
                    }
                }
            }
            return sb.ToString().TrimEnd();
        }
        //Problem 8
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context
                .Addresses
                .OrderByDescending(x => x.Employees.Count())
                .ThenBy(x => x.Town.Name)
                .ThenBy(x => x.AddressText)
                .Select(x => new
                {
                    TownName = x.Town.Name,
                    x.AddressText,
                    Count = x.Employees.Count()
                })
                .Take(10)
                .ToList();

            var sb = new StringBuilder();
            foreach (var address in addresses)
            {
                sb.AppendLine($"{address.AddressText}, {address.TownName} - {address.Count} employees");
            }
            return sb.ToString().TrimEnd();
        }
        //Problem 9
        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees.First(x => x.EmployeeId == 147);
            var sb = new StringBuilder();
            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            var projects = context.Projects.Where(x => x.EmployeesProjects.Any(x => x.EmployeeId == 147)).OrderBy(x=>x.Name).ToList();
            foreach (var item in projects)
            {
                sb.AppendLine(item.Name);
            }
            return sb.ToString().TrimEnd();
        }
        //Problem 10
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context) 
        {
            var departments = context.Departments
                .Where(x => x.Employees.Count() > 5)
                .OrderBy(x=>x.Employees.Count())
                .ThenBy(x=>x.Name)
                .ToList();

            var sb = new StringBuilder();
            foreach (var department in departments)
            {
                var allEmployees = context.Employees.ToList();
                var employees = context.Employees.Where(x => x.DepartmentId == department.DepartmentId).ToList();
                var manager = allEmployees.First(x => x.EmployeeId == department.ManagerId);
                sb.AppendLine($"{department.Name} – {manager.FirstName} {manager.LastName}");
                foreach (var employee in employees)
                {
                    sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 11
        public static string GetLatestProjects(SoftUniContext context) 
        {
            var latestProjects = context
                .Projects.OrderByDescending(x => x.StartDate)
                .Take(10)
                .OrderBy(x=>x.Name)
                .ToList();

            var sb = new StringBuilder();
            foreach (var project in latestProjects)
            {
                sb.AppendLine(project.Name);
                sb.AppendLine(project.Description);
                sb.AppendLine(project.StartDate.ToString());
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 12

        public static string IncreaseSalaries(SoftUniContext context) 
        {
            var employees = context.Employees
                .Where(x => x.Department.Name == "Engineering"
                || x.Department.Name == "Tool Design"
                || x.Department.Name == "Marketing"
                || x.Department.Name == "Information Services")
                .OrderBy(x=>x.FirstName)
                .ThenBy(x=>x.LastName)
                .ToList();

            var sb = new StringBuilder();
            for (int i = 0; i < employees.Count(); i++)
            {
                employees[i].Salary *= 1.12M;
                sb.AppendLine($"{employees[i].FirstName} {employees[i].LastName} (${employees[i].Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 13 66/100
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context) 
        {
            var employees = context.Employees
                .Where(x => x.FirstName.ToLower().StartsWith("sa"))
                .OrderBy(x=>x.FirstName)
                .ThenBy(x=>x.LastName)
                .ToList();

            var sb = new StringBuilder();
            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle} - (${employee.Salary:f2})");
            }
            return sb.ToString().TrimEnd();
        }

        //Problem 14
        public static string DeleteProjectById(SoftUniContext context) 
        {
            var project = context.Projects.First(x => x.ProjectId == 2);
            var employeeProjects = context.EmployeesProjects.Where(x => x.ProjectId == 2).ToList();
            foreach (var employeeProject in employeeProjects)
            {
            context.EmployeesProjects.Remove(employeeProject);
            }
            context.Projects.Remove(project);
            context.SaveChanges();

            var top10Projects = context.Projects.Take(10).Select(x=>new {x.Name }).ToList();
            var sb = new StringBuilder();

            foreach (var item in top10Projects)
            {
                sb.AppendLine(item.Name);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 15
        public static string RemoveTown(SoftUniContext context) 
        {
            var name = "Seattle";
            var town = context.Towns.First(x => x.Name == name);
            var addresses = context.Addresses.Where(x => x.Town.Name == name).ToList();
            var addressCount = addresses.Count();

            var employeesWithThisAddress = context.Employees.Where(x => x.Address.Town.Name == name).ToList();
            foreach (var item in employeesWithThisAddress)
            {
                item.Address = null;
            }

            foreach (var address in addresses)
            {
                context.Addresses.Remove(address);
            }
            context.Towns.Remove(town);
            context.SaveChanges();

            return $"{addressCount} addresses in {name} were deleted";

        }
    }
}


