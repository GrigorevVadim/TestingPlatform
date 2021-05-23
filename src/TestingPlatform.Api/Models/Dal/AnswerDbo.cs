using System;
using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Api.Models.Dal
{
    public class AnswerDbo
    {
        [Key]
        public Guid Id { get; set; }
        
        public string UserAnswer { get; set; }
        
        public string RightAnswer { get; set; }
        
        public Guid QuestionId { get; set; }
        public QuestionDbo Question { get; set; }
        
        public ResultDbo Result { get; set; }
    }
}