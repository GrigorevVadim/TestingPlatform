using System;

namespace TestingPlatform.Api.Models.Dto
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        
        public string Question { get; set; }
        
        public string Answer { get; set; }
    }
}