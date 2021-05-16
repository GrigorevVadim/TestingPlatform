using System;
using System.ComponentModel.DataAnnotations;

namespace TestingPlatform.Api.Models.Dal
{
    public class UserDbo
    {
        [Key]
        public int Id { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }
        
        public Guid Token { get; set; }
        
        public DateTime LastLogIn { get; set; }
    }
}