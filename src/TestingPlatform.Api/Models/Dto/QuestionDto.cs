using System;
using TestingPlatform.Api.Models.Enums;

namespace TestingPlatform.Api.Models.Dto
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        
        public string Question { get; set; }
        
        public string Answer { get; set; }
        
        public EntityState State { get; set; }
    }
}