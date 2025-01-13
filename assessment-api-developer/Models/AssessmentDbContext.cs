using assessment_platform_developer.Models;
using System.Data.Entity;

public class AssessmentDbContext : DbContext
{
    public AssessmentDbContext() : base("AssessmentDbContext")
    {
        Configuration.LazyLoadingEnabled = false; // Explicit includes are required
        Configuration.ProxyCreationEnabled = false;
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<Country> Countries { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relationships
        modelBuilder.Entity<Customer>()
            .HasRequired(c => c.State)
            .WithMany()
            .HasForeignKey(c => c.StateID)
            .WillCascadeOnDelete(false);

        modelBuilder.Entity<State>()
            .HasRequired(s => s.Country)
            .WithMany(c => c.States)
            .HasForeignKey(s => s.CountryID)
            .WillCascadeOnDelete(false);
    }
}