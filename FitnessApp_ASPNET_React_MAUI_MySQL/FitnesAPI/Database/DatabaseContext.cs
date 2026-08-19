using Microsoft.EntityFrameworkCore;

namespace FitnesAPI.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> context):base(context) { 
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Signup> Signups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Signup>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Signup>()
            .HasOne<Class>()
            .WithMany()
            .HasForeignKey(s => s.ClassId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Signup>()
            .HasIndex(s => new { s.UserId, s.ClassId })
            .IsUnique();

        modelBuilder.Entity<Class>()
            .Property(c => c.Repetition)
            .HasConversion<string>();
        
        base.OnModelCreating(modelBuilder);
    }
}