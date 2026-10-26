using Microsoft.EntityFrameworkCore;
using Orbital.Model;

namespace Orbital.Context
{
    public class CrewContext : DbContext
    {
        // Don't change this variable.
        private readonly string _defaultConnectionString = "Data Source=../../../DB/crew.db";

        public DbSet<CrewMember> CrewMembers { get; set; } = null!;

        // Dont change this constructor.
        public CrewContext(DbContextOptions<CrewContext> options) : base(options)
        {
        }

        // Dont change this constructor.
        public CrewContext()
        {
        }

        // Don't change this override.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(_defaultConnectionString);
            }
        }
    }
}
