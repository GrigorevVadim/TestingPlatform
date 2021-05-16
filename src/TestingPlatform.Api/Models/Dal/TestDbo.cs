using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Api.Models.Dal
{
    public class TestDbo
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public List<QuestionDbo> Questions { get; set; }
        
        public UserDbo Owner { get; set; }
    }
}