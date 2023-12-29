using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Taski.Api.Entities;
using Microsoft.AspNetCore.Identity;


namespace Taski.Api.Data;

public class TaskiAppContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public TaskiAppContext(DbContextOptions<TaskiAppContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Story>()
            .HasOne(s => s.Project)
            .WithMany(p => p.Stories)
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Story>()
            .HasOne(s => s.CreatedByUser)
            .WithMany(u => u.CreatedStories)
            .HasForeignKey(s => s.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Story>()
            .HasOne(s => s.AssignedToUser)
            .WithMany(u => u.AssignedStories)
            .HasForeignKey(s => s.AssignedTo)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Project>()
            .HasOne(p => p.User)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Story> Stories => Set<Story>();
    public DbSet<Role> Roles => Set<Role>();
}