using Microsoft.EntityFrameworkCore;
using TestingPlatform.Api.Models.Dal;

namespace TestingPlatform.Api.Models
{
    public class ModelsDataContext : DbContext
    {
        public ModelsDataContext(DbContextOptions<ModelsDataContext> options) : base(options)
        {
        }
        public DbSet<UserDbo> Users { get; set; }
    }
}