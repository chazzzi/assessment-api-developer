using System.Data.Entity;

namespace assessment_platform_developer.Models
{
    public class AssessmentDbContext : DbContext
    {
        public AssessmentDbContext()
            : base("AssessmentDbContext") 
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
