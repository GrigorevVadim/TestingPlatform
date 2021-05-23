using System;

namespace TestingPlatform.Api.Models.Dto
{
    public class ResultDto
    {
        public Guid Id { get; set; }
        
        public DateTime DateTime { get; set; }
        
        public double Score { get; set; }
        
        public string UserLogin { get; set; }
    }
}