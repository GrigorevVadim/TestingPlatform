using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Api.Models.Dal
{
    public class ResultDbo
    {
        [Key]
        public Guid Id { get; set; }
        
        public DateTime DateTime { get; set; }
        
        public double Score { get; set; }
        
        public UserDbo User { get; set; }
        
        public TestDbo Test { get; set; }
        
        public List<AnswerDbo> Answers { get; set; }
    }
}