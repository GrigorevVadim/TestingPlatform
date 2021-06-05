using System;
using TestingPlatform.Api.Models.Enums;

namespace TestingPlatform.Api.Models.Dto
{
    public class TestDto
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public EntityState State { get; set; }
    }
}