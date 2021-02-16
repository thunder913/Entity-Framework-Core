using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();
            //Console.WriteLine("Hello World!");
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
            //Console.WriteLine(GetEmployeesByFirstNameStartingWithSa(context));
            //Console.WriteLine(DeleteProjectById(context));
            Console.WriteLine(RemoveTown(context));
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                            .Select(e => new
                            {
                                e.FirstName,
                                e.LastName,
                                e.MiddleName,
                                e.JobTitle,
                                e.Salary,
                                e.EmployeeId
                            })
                            .OrderBy(x => x.EmployeeId)
                            .ToList();
            var sb = new StringBuilder();
            foreach (var item in employees)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} {item.MiddleName} {item.JobTitle} {item.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                    .Where(e => e.Salary > 50000)
                    .Select(e => new
                    {
                        e.Salary,
                        e.FirstName
                    })
                    .OrderBy(e => e.FirstName)
                    .ToList();

            var sb = new StringBuilder();
            foreach (var item in employees)
            {
                sb.AppendLine($"{item.FirstName} - {item.Salary:f2}");
            }
            return sb.ToString();
        }
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                                .Where(e => e.Department.Name == "Research and Development")
                                .OrderBy(e => e.Salary)
                                .ThenByDescending(e => e.FirstName)
                                .Select(e => new
                                {
                                    e.FirstName,
                                    e.LastName,
                                    DepartName = e.Department.Name,
                                    e.Salary
                                })
                                .ToList();

            var sb = new StringBuilder();
            foreach (var item in employees)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} from {item.DepartName} - ${item.Salary:f2}");
            }
            return sb.ToString();
        }
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var newAddress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };
            context.Addresses.Add(newAddress);

            var nakovPeople = context.Employees
                        .Where(e => e.LastName == "Nakov")
                        .ToList();

            for (int i = 0; i < nakovPeople.Count(); i++)
            {
                nakovPeople[i].Address = newAddress;
            }

            context.SaveChanges();

            var addressTexts = context.Employees
                            .OrderByDescending(e => e.AddressId)
                            .Select(e => new
                            {
                                e.Address.AddressText
                            })
                            .Take(10)
                            .ToList();

            var sb = new StringBuilder();
            foreach (var item in addressTexts)
            {
                sb.AppendLine(item.AddressText);
            }
            return sb.ToString();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                                .Where(e => e.EmployeesProjects.
                                        Where(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003).Count() > 0)
                                .Select(e => new
                                {
                                    e.EmployeeId,
                                    e.FirstName,
                                    e.LastName,
                                    ManagerFirstName = e.Manager.FirstName,
                                    ManagerLastName = e.Manager.LastName,
                                    Projects = context.EmployeesProjects.Where(ep => ep.EmployeeId == e.EmployeeId)
                                                        .Select(ep => new
                                                        {
                                                            ep.Project.Name,
                                                            ep.Project.StartDate,
                                                            ep.Project.EndDate
                                                        })
                                                        .ToList()
                                })
                                .Take(10)
                                .ToList();

            var sb = new StringBuilder();
            foreach (var empl in employees)
            {
                sb.AppendLine($"{empl.FirstName} {empl.LastName} - Manager: {empl.ManagerFirstName} {empl.ManagerLastName}");

                foreach (var proj in empl.Projects)
                {
                    string endDate = proj.EndDate != null ? proj.EndDate.ToString() : "not finished";
                    sb.AppendLine($"--{proj.Name} - {proj.StartDate} - {endDate}");
                }
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                                .OrderByDescending(a => a.Employees.Count())
                                .ThenBy(a => a.Town.Name)
                                .ThenBy(a => a.AddressText)
                                .Select(a => new
                                {
                                    a.AddressText,
                                    TownName = a.Town.Name,
                                    EmployeeCount = a.Employees.Count()
                                })
                                .Take(10)
                                .ToList();
            var sb = new StringBuilder();
            foreach (var addr in addresses)
            {
                sb.AppendLine($"{addr.AddressText}, {addr.TownName} - {addr.EmployeeCount} employees");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee147 = context.Employees
                                .Where(e => e.EmployeeId == 147)
                                .Select(e => new
                                {
                                    e.FirstName,
                                    e.LastName,
                                    e.JobTitle,
                                    Projects = context.EmployeesProjects
                                                .Where(ep => ep.EmployeeId == 147)
                                                .OrderBy(ep => ep.Project.Name)
                                                .Select(ep => new
                                                {
                                                    projectName = ep.Project.Name
                                                })
                                                .ToList()
                                })
                                .First();
            var sb = new StringBuilder();
            sb.AppendLine($"{employee147.FirstName} {employee147.LastName} - {employee147.JobTitle}");
            foreach (var proj in employee147.Projects)
            {
                sb.AppendLine(proj.projectName);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                                .Where(d => d.Employees.Count() > 5)
                                .OrderBy(d => d.Employees.Count())
                                .ThenBy(d => d.Name)
                                .Select(d => new
                                {
                                    d.Name,
                                    managerFIrstName = d.Manager.FirstName,
                                    managerLastName = d.Manager.LastName,
                                    employees = d.Employees
                                            .OrderBy(e => e.FirstName)
                                            .ThenBy(e => e.LastName)
                                            .Select(e => new
                                            {
                                                e.FirstName,
                                                e.LastName,
                                                e.JobTitle
                                            })
                                            .ToList()
                                })
                                .ToList();
            var sb = new StringBuilder();

            foreach (var dep in departments)
            {
                sb.AppendLine($"{dep.Name} - {dep.managerFIrstName} {dep.managerLastName}");
                foreach (var empl in dep.employees)
                {
                    sb.AppendLine($"{empl.FirstName} {empl.LastName} - {empl.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                            .Where(p => p.EndDate == null)
                            .OrderByDescending(p => p.StartDate)
                            .Take(10)
                            .Select(p => new
                            {
                                p.Name,
                                p.Description,
                                p.StartDate
                            })
                            .ToList()
                            .OrderBy(x => x.Name)
                            .ThenBy(x => x.Description)
                            .ThenBy(x => x.StartDate)
                            .ToList();

            var sb = new StringBuilder();
            foreach (var proj in projects)
            {
                sb.AppendLine(proj.Name);
                sb.AppendLine(proj.Description);
                sb.AppendLine(proj.StartDate.ToString("M/d/yyyy h:mm:ss tt"));
            }
            return sb.ToString();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employees = context.Employees
                                        .Where(e => e.Department.Name == "Engineering"
                                            || e.Department.Name == "Tool Design"
                                            || e.Department.Name == "Marketing"
                                            || e.Department.Name == "Information Services")
                                        .OrderBy(e => e.FirstName)
                                        .ThenBy(e => e.LastName)
                                        .ToList();
            var sb = new StringBuilder();

            for (int i = 0; i < employees.Count(); i++)
            {
                employees[i].Salary *= 1.12M;
                sb.AppendLine($"{employees[i].FirstName} {employees[i].LastName} (${employees[i].Salary:f2})");
            }
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                                        .Where(e => e.FirstName.StartsWith("Sa"))
                                        .OrderBy(e => e.FirstName)
                                        .ThenBy(e => e.LastName)
                                        .Select(e => new
                                        {
                                            e.FirstName,
                                            e.LastName,
                                            e.JobTitle,
                                            e.Salary
                                        })
                                        .ToList();
            var sb = new StringBuilder();
            foreach (var empl in employees)
            {
                sb.AppendLine($"{empl.FirstName} {empl.LastName} - {empl.JobTitle} - (${empl.Salary:f2})");
            }
            return sb.ToString().TrimEnd();
        }
        public static string DeleteProjectById(SoftUniContext context)
        {
            var employeeProjects = context.EmployeesProjects
                                        .Where(ep => ep.ProjectId == 2)
                                        .ToList();
            context.EmployeesProjects.RemoveRange(employeeProjects);

            var projects = context
                            .Projects
                                .Where(p => p.ProjectId == 2)
                                    .ToList();
            context.Projects.RemoveRange(projects);

            context.SaveChanges();

            var selectedProjects = context.Projects
                                        .Select(p => new
                                        {
                                            p.Name
                                        })
                                        .Take(10)
                                        .ToList();

            var sb = new StringBuilder();
            foreach (var proj in selectedProjects)
            {
                sb.AppendLine(proj.Name);
            }
            return sb.ToString().TrimEnd();
        }

        public static string RemoveTown(SoftUniContext context)
        {
            var employees = context
                                .Employees
                                .Where(e => e.Address.Town.Name == "Seattle")
                                .ToList();
            foreach (var empl in employees)
            {
                empl.Address = null;
            }

            var addresses = context
                                .Addresses
                                .Where(e => e.Town.Name == "Seattle")
                                .ToList();

            var removedAddresses = addresses.Count();

            context.Addresses.RemoveRange(addresses);


            var seattle = context
                            .Towns
                            .Where(t => t.Name == "Seattle")
                            .FirstOrDefault();

            context.Towns.Remove(seattle);

            context.SaveChanges();

            return $"{removedAddresses} addresses in Seattle were deleted";

        }
    }
}
