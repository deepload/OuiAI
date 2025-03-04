using Microsoft.EntityFrameworkCore;
using OuiAI.Microservices.Projects.Models;

namespace OuiAI.Microservices.Projects.Data
{
    public class ProjectsDbContext : DbContext
    {
        public ProjectsDbContext(DbContextOptions<ProjectsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectCategory> Categories { get; set; }
        public DbSet<ProjectTag> Tags { get; set; }
        public DbSet<ProjectMedia> Media { get; set; }
        public DbSet<ProjectLike> Likes { get; set; }
        public DbSet<ProjectComment> Comments { get; set; }
        public DbSet<ProjectView> Views { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure project entity
            modelBuilder.Entity<Project>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Projects)
                .HasForeignKey(p => p.CategoryId);

            // Configure project likes
            modelBuilder.Entity<ProjectLike>()
                .HasKey(pl => new { pl.ProjectId, pl.UserId });

            modelBuilder.Entity<ProjectLike>()
                .HasOne(pl => pl.Project)
                .WithMany(p => p.Likes)
                .HasForeignKey(pl => pl.ProjectId);

            // Configure project views
            modelBuilder.Entity<ProjectView>()
                .HasKey(pv => new { pv.ProjectId, pv.UserId });

            modelBuilder.Entity<ProjectView>()
                .HasOne(pv => pv.Project)
                .WithMany(p => p.Views)
                .HasForeignKey(pv => pv.ProjectId);

            // Configure project comments
            modelBuilder.Entity<ProjectComment>()
                .HasKey(pc => pc.Id);

            modelBuilder.Entity<ProjectComment>()
                .HasOne(pc => pc.Project)
                .WithMany(p => p.Comments)
                .HasForeignKey(pc => pc.ProjectId);

            modelBuilder.Entity<ProjectComment>()
                .HasOne(pc => pc.ParentComment)
                .WithMany(pc => pc.Replies)
                .HasForeignKey(pc => pc.ParentCommentId)
                .IsRequired(false);

            // Configure project media
            modelBuilder.Entity<ProjectMedia>()
                .HasKey(pm => pm.Id);

            modelBuilder.Entity<ProjectMedia>()
                .HasOne(pm => pm.Project)
                .WithMany(p => p.Media)
                .HasForeignKey(pm => pm.ProjectId);

            // Configure project tags
            modelBuilder.Entity<ProjectTag>()
                .HasKey(pt => pt.Id);

            modelBuilder.Entity<ProjectTagRelation>()
                .HasKey(ptr => new { ptr.ProjectId, ptr.TagId });

            modelBuilder.Entity<ProjectTagRelation>()
                .HasOne(ptr => ptr.Project)
                .WithMany(p => p.TagRelations)
                .HasForeignKey(ptr => ptr.ProjectId);

            modelBuilder.Entity<ProjectTagRelation>()
                .HasOne(ptr => ptr.Tag)
                .WithMany(t => t.ProjectRelations)
                .HasForeignKey(ptr => ptr.TagId);
        }
    }
}
