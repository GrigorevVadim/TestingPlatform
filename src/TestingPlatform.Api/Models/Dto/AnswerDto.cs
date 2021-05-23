using System;

namespace TestingPlatform.Api.Models.Dto
{
    public class AnswerDto
    {
        public string UserAnswer { get; set; }
        
        public string RightAnswer { get; set; }
        
        public Guid QuestionId { get; set; }
    }
}