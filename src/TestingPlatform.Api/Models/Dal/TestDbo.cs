using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TestingPlatform.Api.Models.Enums;

namespace TestingPlatform.Api.Models.Dal
{
    public class TestDbo
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public List<QuestionDbo> Questions { get; set; }
        
        public UserDbo Owner { get; set; }
        
        public EntityState State { get; set; }
    }
}