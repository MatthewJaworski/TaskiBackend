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
        modelBuilder.Entity<Comment>()
            .HasOne(c=>c.Story)
            .WithMany(s=>s.Comments)
            .HasForeignKey(c=>c.StoryId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany() 
            .HasForeignKey(c => c.UserId);
        modelBuilder.Entity<Project>()
            .HasOne(p => p.User)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProjectTagAssociation>()
            .HasKey(pta => new { pta.ProjectId, pta.ProjectTagId });

        modelBuilder.Entity<ProjectTagAssociation>()
            .HasOne(pta => pta.Project)
            .WithMany(p => p.TagAssociations)
            .HasForeignKey(pta => pta.ProjectId);

        modelBuilder.Entity<ProjectTagAssociation>()
            .HasOne(pta => pta.ProjectTag)
            .WithMany(pt => pt.ProjectAssociations)
            .HasForeignKey(pta => pta.ProjectTagId);

        modelBuilder.Entity<Story>()
            .HasOne(s => s.Tag)
            .WithMany(t => t.Stories)
            .HasForeignKey(s => s.TagId)
            .IsRequired(false);

        modelBuilder.Entity<UserProjectAssociation>()
            .HasKey(up => new { up.Id });

        modelBuilder.Entity<UserProjectAssociation>()
            .HasOne(up => up.User)
            .WithMany(u => u.UserProjectAssociations)
            .HasForeignKey(up => up.UserId);

        modelBuilder.Entity<UserProjectAssociation>()
            .HasOne(up => up.Project)
            .WithMany(p => p.UserProjectAssociations)
            .HasForeignKey(up => up.ProjectId);
    }


    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Story> Stories => Set<Story>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<ProjectTag> ProjectTags => Set<ProjectTag>();
    public DbSet<StoryTag> StoryTags => Set<StoryTag>();
    public DbSet<ProjectTagAssociation> ProjectTagAssociations => Set<ProjectTagAssociation>();
    public DbSet<UserProjectAssociation> UserProjectAssociations => Set<UserProjectAssociation>();
}