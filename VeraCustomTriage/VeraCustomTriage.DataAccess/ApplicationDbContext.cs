using Microsoft.EntityFrameworkCore;
using VeraCustomTriage.Shared.Models;

namespace VeraCustomTriage.DataAccess.Mssql
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<AutoResponse> AutoResponse { get; set; }
        public DbSet<PropertyCondition> PropertyCondition { get; set; }
        public DbSet<Template> Templates { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
        { }
    }
}
