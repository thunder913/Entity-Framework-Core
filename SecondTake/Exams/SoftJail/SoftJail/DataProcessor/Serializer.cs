namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners =
                context
                .Prisoners
                .Where(p => ids.Contains(p.Id))
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.Id)
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers.Select(x => new
                    {
                        OfficerName = x.Officer.FullName,
                        Department = x.Officer.Department.Name
                    })
                    .OrderBy(y => y.OfficerName)
                    .ToArray(),
                    TotalOfficerSalary = Math.Round(p.PrisonerOfficers.Sum(y => y.Officer.Salary), 2)
                })
                .ToArray();

            var jsonText = JsonConvert.SerializeObject(prisoners, Newtonsoft.Json.Formatting.Indented);
            return jsonText;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var names = prisonersNames.Split(",");

            var prisoners =
                context
                .Prisoners
                .Where(x => names.Contains(x.FullName))
                .Select(p => new PrisonerExport()
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd"),
                    Messages = p.Mails.Select(m => new Message()
                    {
                        Description = m.Description
                    })
                    .ToArray()
                })
                .OrderBy(x=>x.Name)
                .ThenBy(x=>x.Id)
                .ToArray();

            for (int i = 0; i < prisoners.Count(); i++)
            {
                for (int j = 0; j < prisoners[i].Messages.Count(); j++)
                {
                    prisoners[i].Messages[j].Description = new string(prisoners[i].Messages[j].Description.Reverse().ToArray());
                }
            }

            var serializer = new XmlSerializer(typeof(PrisonerExport[]), new XmlRootAttribute("Prisoners"));

            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), prisoners, new XmlSerializerNamespaces(new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new XmlQualifiedName(string.Empty)
            })));
            return sb.ToString().TrimEnd();
        }
    }
}