using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    public class Student
    {
        public Student() 
        {
           this.HomeworkSubmissions = new HashSet<Homework>();
           this.CourseEnrollments = new HashSet<StudentCourse>();
        }
        [Key] [Required]
        public int StudentId { get; set; }
        
        [MaxLength(100)][Required]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "varchar(10)")]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }
   
        public DateTime? Birthday { get; set; }

        [Required]
        public DateTime RegisteredOn { get; set; }

        public virtual ICollection<Homework> HomeworkSubmissions { get; set; }
        public virtual ICollection<StudentCourse> CourseEnrollments { get; set; }
    }
}
