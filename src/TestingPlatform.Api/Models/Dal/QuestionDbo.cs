using System;
using System.ComponentModel.DataAnnotations;
using TestingPlatform.Api.Models.Enums;

namespace TestingPlatform.Api.Models.Dal
{
    public class QuestionDbo
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Question { get; set; }
        
        public string Answer { get; set; }
        
        public TestDbo Test { get; set; }
        
        public EntityState State { get; set; }
    }
}