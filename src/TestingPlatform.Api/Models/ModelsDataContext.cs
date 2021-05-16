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

        public DbSet<TestDbo> Tests { get; set; }

        public DbSet<QuestionDbo> Questions { get; set; }
    }
}