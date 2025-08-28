using IceSyncApp.Components.Models;
using Microsoft.EntityFrameworkCore;

namespace IceSyncApp.Components.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        public DbSet<Workflow> Workflows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Workflow entity
            modelBuilder.Entity<Workflow>(entity =>
            {
                entity.HasKey(w => w.WorkflowId);

                entity.Property(w => w.WorkflowId)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(w => w.WorkflowName)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(w => w.IsActive)
                      .IsRequired();

                entity.Property(w => w.MultiExecBehavior)
                      .HasMaxLength(100);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
