namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var projects =
                context
                .Projects
                .Include(x => x.Tasks)
                .ToArray()
                .Where(p => p.Tasks.Any())
                .Select(p => new ProjectExportDto()
                {
                    ProjectName = p.Name,
                    HasEndDate = p.DueDate == null ? "No" : "Yes",
                    Tasks = p.Tasks.Select(t => new TaskExportDto()
                    {
                        Label = Enum.GetName(typeof(LabelType), t.LabelType),
                        Name = t.Name,
                    })
                    .OrderBy(t => t.Name)
                    .ToArray(),
                    TasksCount = p.Tasks.Count()
                })
                .OrderByDescending(p => p.TasksCount)
                .ThenBy(p => p.ProjectName)
                .ToArray();


            var serializer = new XmlSerializer(typeof(ProjectExportDto[]), new XmlRootAttribute("Projects"));
            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), projects, new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") }));

            return sb.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees =
                context
                .Employees
                .Include(x => x.EmployeesTasks)
                .ThenInclude(x => x.Task)
                .ToArray()
                .Where(x => x.EmployeesTasks.Any(y => y.Task.OpenDate >= date))
                .OrderByDescending(e => e.EmployeesTasks.Count(y => y.Task.OpenDate >= date))
                .ThenBy(e => e.Username)
                .Take(10)
                .Select(e => new
                {
                    Username = e.Username,
                    Tasks = e.EmployeesTasks
                    .Where(et => et.Task.OpenDate >= date)
                    .OrderByDescending(et => et.Task.DueDate)
                    .ThenBy(et => et.Task.Name)
                    .Select(et => new
                    {
                        TaskName = et.Task.Name,
                        OpenDate = et.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = et.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        LabelType = Enum.GetName(typeof(LabelType), et.Task.LabelType),
                        ExecutionType = Enum.GetName(typeof(ExecutionType), et.Task.ExecutionType),
                    })
                })
                .ToList();

            var jsonText = JsonConvert.SerializeObject(employees, Formatting.Indented);

            return jsonText;
                
        }
    }
}