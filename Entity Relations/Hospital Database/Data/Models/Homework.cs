using P01_StudentSystem.Data.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    public class Homework
    {
        public int HomeworkId { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        [Required]
        public string Content { get; set; }
        [Required]
        public ContentType ContentType { get; set; }

        [Required]
        public DateTime SubmissionTime { get; set; }
        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; }
        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
