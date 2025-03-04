using Microsoft.EntityFrameworkCore;
using OuiAI.Microservices.Social.Models;

namespace OuiAI.Microservices.Social.Data
{
    public class SocialDbContext : DbContext
    {
        public SocialDbContext(DbContextOptions<SocialDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserFollowModel> UserFollows { get; set; }
        public DbSet<NotificationModel> Notifications { get; set; }
        public DbSet<ActivityModel> Activities { get; set; }
        public DbSet<ConversationModel> Conversations { get; set; }
        public DbSet<ConversationParticipantModel> ConversationParticipants { get; set; }
        public DbSet<MessageModel> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure UserFollow entity
            modelBuilder.Entity<UserFollowModel>()
                .HasKey(uf => new { uf.FollowerId, uf.FolloweeId });

            // Configure Notification entity
            modelBuilder.Entity<NotificationModel>()
                .HasKey(n => n.Id);

            // Configure Activity entity
            modelBuilder.Entity<ActivityModel>()
                .HasKey(a => a.Id);

            // Configure Conversation entity
            modelBuilder.Entity<ConversationModel>()
                .HasKey(c => c.Id);

            // Configure ConversationParticipant entity
            modelBuilder.Entity<ConversationParticipantModel>()
                .HasKey(cp => new { cp.ConversationId, cp.UserId });

            modelBuilder.Entity<ConversationParticipantModel>()
                .HasOne(cp => cp.Conversation)
                .WithMany(c => c.Participants)
                .HasForeignKey(cp => cp.ConversationId);

            // Configure Message entity
            modelBuilder.Entity<MessageModel>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<MessageModel>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId);

            // Configure Message.AttachmentUrls as a JSON column
            modelBuilder.Entity<MessageModel>()
                .Property(m => m.AttachmentUrls)
                .HasConversion(
                    v => string.Join(";", v),
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());
        }
    }
}
