﻿using P01_StudentSystem.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    public class Resource
    {
        public int ResourceId { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
        [Required]
        public ResourceType ResourceType { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        [Required]
        public string Url { get; set; }
        public Course Course { get; set; }
        [Required]
        public int CourseId { get; set; }
    }
}
