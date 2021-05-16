using System;
using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Api.Models.Dal
{
    public class QuestionDbo
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Question { get; set; }
        
        public string Answer { get; set; }
        
        public TestDbo Test { get; set; }
    }
}