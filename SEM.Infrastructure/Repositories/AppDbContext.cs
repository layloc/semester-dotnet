using Microsoft.EntityFrameworkCore;
using SEM.Entities;

namespace SEM.Infrastructure.Repositories;

public class AppDbContext : DbContext
{
    public DbSet<Model> Models { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<PropertyInfo> PropertyInfos { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.SavedModels)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId);
        modelBuilder.Entity<Model>()
            .HasMany(m => m.Properties)
            .WithOne(p => p.Model)
            .HasForeignKey(m => m.ModelId);
    }
}