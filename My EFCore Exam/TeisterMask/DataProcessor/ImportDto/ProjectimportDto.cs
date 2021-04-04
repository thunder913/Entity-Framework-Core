using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Project")]
    public class ProjectimportDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Name { get; set; }

        public string OpenDate { get; set; }

        public string DueDate { get; set; }

        [XmlArray("Tasks")]
        public TaskImportDto[] Tasks { get; set; }
    }

    [XmlType("Task")]
    public class TaskImportDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Name { get; set; }

        [Required]
        public string OpenDate { get; set; }

        [Required]
        public string DueDate { get; set; }

        public int ExecutionType { get; set; }

        public int LabelType { get; set; }
    }
}
