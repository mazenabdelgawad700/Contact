using Contact.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Contact.Infrastructure.Context
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<User> User { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<CommentMedia> CommentMedia { get; set; }
        public DbSet<CommentReaction> CommentReaction { get; set; }
        public DbSet<Friendship> Friendship { get; set; }
        public DbSet<Hashtag> Hashtag { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<PostHashtag> PostHashtag { get; set; }
        public DbSet<PostMedia> PostMedia { get; set; }
        public DbSet<PostMention> PostMention { get; set; }
        public DbSet<PostReaction> PostReaction { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.UserName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.FriendsCount).HasDefaultValue(0);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(u => u.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                // One-to-One relationship with UserProfile
                entity.HasOne(u => u.UserProfile)
                      .WithOne(up => up.User)
                      .HasForeignKey<UserProfile>(up => up.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // UserProfile Configuration
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(up => up.Id);
                entity.Property(up => up.Id).IsRequired();
                entity.Property(up => up.UserId).IsRequired();
                entity.Property(up => up.Bio).HasMaxLength(500);
                entity.Property(up => up.ProfilePhotoUrl).HasMaxLength(500);
                entity.Property(up => up.IsProfilePublic).HasDefaultValue(true);
                entity.Property(up => up.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(up => up.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Unique constraint on UserId
                entity.HasIndex(up => up.UserId).IsUnique();
            });

            // Post Configuration
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Content).HasMaxLength(2000);
                entity.Property(p => p.PrivacyLevel).HasDefaultValue(0);
                entity.Property(p => p.LikesCount).HasDefaultValue(0);
                entity.Property(p => p.CommentsCount).HasDefaultValue(0);
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(p => p.IsDeleted).HasDefaultValue(false);
                entity.Property(p => p.UserId).IsRequired();

                // Many-to-One relationship with User
                entity.HasOne(p => p.User)
                      .WithMany(u => u.Posts)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Indexes for performance
                entity.HasIndex(p => p.CreatedAt);
                entity.HasIndex(p => p.UserId);
                entity.HasIndex(p => p.IsDeleted);
            });

            // PostMedia Configuration
            modelBuilder.Entity<PostMedia>(entity =>
            {
                entity.HasKey(pm => pm.Id);
                entity.Property(pm => pm.MediaUrl).IsRequired().HasMaxLength(500);
                entity.Property(pm => pm.MediaType).IsRequired().HasMaxLength(50);
                entity.Property(pm => pm.MediaOrder).HasDefaultValue(0);
                entity.Property(pm => pm.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Many-to-One relationship with Post
                entity.HasOne(pm => pm.Post)
                      .WithMany(p => p.PostMedia)
                      .HasForeignKey(pm => pm.PostId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(pm => pm.PostId);
            });

            // PostMention Configuration
            modelBuilder.Entity<PostMention>(entity =>
            {
                entity.HasKey(pm => pm.Id);
                entity.Property(pm => pm.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Many-to-One relationship with Post
                entity.HasOne(pm => pm.Post)
                      .WithMany(p => p.PostMentions)
                      .HasForeignKey(pm => pm.PostId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Many-to-One relationship with User (Mentioned User)
                entity.HasOne(pm => pm.MentionedUser)
                      .WithMany(u => u.PostMentions)
                      .HasForeignKey(pm => pm.MentionedUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Prevent duplicate mentions in same post
                entity.HasIndex(pm => new { pm.PostId, pm.MentionedUserId }).IsUnique();
            });

            // Hashtag Configuration
            modelBuilder.Entity<Hashtag>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.Property(h => h.Name).IsRequired().HasMaxLength(100);
                entity.Property(h => h.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(h => h.UsageCount).HasDefaultValue(0);

                // Unique constraint on hashtag name (case-insensitive)
                entity.HasIndex(h => h.Name).IsUnique();
            });

            // PostHashtag Configuration (Many-to-Many)
            modelBuilder.Entity<PostHashtag>(entity =>
            {
                entity.HasKey(ph => ph.Id);
                entity.Property(ph => ph.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Many-to-One relationship with Post
                entity.HasOne(ph => ph.Post)
                      .WithMany(p => p.PostHashtags)
                      .HasForeignKey(ph => ph.PostId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Many-to-One relationship with Hashtag
                entity.HasOne(ph => ph.Hashtag)
                      .WithMany(h => h.PostHashtags)
                      .HasForeignKey(ph => ph.HashtagId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Prevent duplicate hashtags in same post
                entity.HasIndex(ph => new { ph.PostId, ph.HashtagId }).IsUnique();
            });

            // PostReaction Configuration
            modelBuilder.Entity<PostReaction>(entity =>
            {
                entity.HasKey(pr => pr.Id);
                entity.Property(pr => pr.ReactionType).IsRequired();
                entity.Property(pr => pr.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(pr => pr.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Many-to-One relationship with Post
                entity.HasOne(pr => pr.Post)
                      .WithMany(p => p.PostReactions)
                      .HasForeignKey(pr => pr.PostId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Many-to-One relationship with User
                entity.HasOne(pr => pr.User)
                      .WithMany(u => u.PostReactions)
                      .HasForeignKey(pr => pr.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // One reaction per user per post
                entity.HasIndex(pr => new { pr.PostId, pr.UserId }).IsUnique();
            });

            // Comment Configuration
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Content).HasMaxLength(1000);
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(c => c.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(c => c.IsDeleted).HasDefaultValue(false);

                // Many-to-One relationship with Post
                entity.HasOne(c => c.Post)
                      .WithMany(p => p.Comments)
                      .HasForeignKey(c => c.PostId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Many-to-One relationship with User
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Self-referencing relationship for replies
                entity.HasOne(c => c.ParentComment)
                      .WithMany(c => c.Replies)
                      .HasForeignKey(c => c.ParentCommentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => c.PostId);
                entity.HasIndex(c => c.UserId);
                entity.HasIndex(c => c.ParentCommentId);
            });

            // CommentMedia Configuration
            modelBuilder.Entity<CommentMedia>(entity =>
            {
                entity.HasKey(cm => cm.Id);
                entity.Property(cm => cm.MediaUrl).IsRequired().HasMaxLength(500);
                entity.Property(cm => cm.MediaType).IsRequired().HasMaxLength(50);
                entity.Property(cm => cm.MediaOrder).HasDefaultValue(0);
                entity.Property(cm => cm.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Many-to-One relationship with Comment
                entity.HasOne(cm => cm.Comment)
                      .WithMany(c => c.CommentMedia)
                      .HasForeignKey(cm => cm.CommentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(cm => cm.CommentId);
            });

            // CommentReaction Configuration
            modelBuilder.Entity<CommentReaction>(entity =>
            {
                entity.HasKey(cr => cr.Id);
                entity.Property(cr => cr.ReactionType).IsRequired();
                entity.Property(cr => cr.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(cr => cr.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Many-to-One relationship with Comment
                entity.HasOne(cr => cr.Comment)
                      .WithMany(c => c.CommentReactions)
                      .HasForeignKey(cr => cr.CommentId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Many-to-One relationship with User
                entity.HasOne(cr => cr.User)
                      .WithMany(u => u.CommentReactions)
                      .HasForeignKey(cr => cr.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // One reaction per user per comment
                entity.HasIndex(cr => new { cr.CommentId, cr.UserId }).IsUnique();
            });

            // Friendship Configuration
            modelBuilder.Entity<Friendship>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Status).IsRequired();
                entity.Property(f => f.RequestedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(f => f.RespondedAt);

                // Many-to-One relationship with User (Requester)
                entity.HasOne(f => f.Requester)
                      .WithMany(u => u.SentFriendRequests)
                      .HasForeignKey(f => f.RequesterId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Many-to-One relationship with User (Receiver)
                entity.HasOne(f => f.Receiver)
                      .WithMany(u => u.ReceivedFriendRequests)
                      .HasForeignKey(f => f.ReceiverId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Prevent duplicate friendship requests
                entity.HasIndex(f => new { f.RequesterId, f.ReceiverId }).IsUnique();

                // Check constraint to prevent self-friendship
                entity.HasCheckConstraint("CK_Friendship_NoSelfFriend", "RequesterId != ReceiverId");
            });

            // Notification Configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.NotificationType).IsRequired();
                entity.Property(n => n.RelatedEntityId).HasMaxLength(50);
                entity.Property(n => n.ActorUserId).IsRequired(false); // Make nullable to allow SET NULL
                entity.Property(n => n.Content).IsRequired().HasMaxLength(500);
                entity.Property(n => n.IsRead).HasDefaultValue(false);
                entity.Property(n => n.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Many-to-One relationship with User (Recipient)
                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Many-to-One relationship with User (Actor - who triggered the notification)
                entity.HasOne(n => n.ActorUser)
                      .WithMany()
                      .HasForeignKey(n => n.ActorUserId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(n => n.UserId);
                entity.HasIndex(n => n.CreatedAt);
                entity.HasIndex(n => n.IsRead);
            });

            // RefreshToken Configuration
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);
                entity.Property(rt => rt.UserId).IsRequired();
                entity.Property(rt => rt.Token).HasMaxLength(500);
                entity.Property(rt => rt.JwtId).HasMaxLength(200);
                entity.Property(rt => rt.IsUsed).HasDefaultValue(false);
                entity.Property(rt => rt.IsRevoked).HasDefaultValue(false);
                entity.Property(rt => rt.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(rt => rt.ExpiresAt).IsRequired();

                // Many-to-One relationship with User
                entity.HasOne(rt => rt.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(rt => rt.Token);
                entity.HasIndex(rt => rt.UserId);
                entity.HasIndex(rt => rt.JwtId);
            });

            // Additional Configurations for better performance and data integrity

            // Configure cascade updates for counter fields
            // Note: You'll need to handle these in your business logic or use triggers

            // Indexes for frequently queried fields
            modelBuilder.Entity<Post>()
                .HasIndex(p => new { p.UserId, p.CreatedAt })
                .HasDatabaseName("IX_Post_UserId_CreatedAt");

            modelBuilder.Entity<Comment>()
                .HasIndex(c => new { c.PostId, c.CreatedAt })
                .HasDatabaseName("IX_Comment_PostId_CreatedAt");

            modelBuilder.Entity<Notification>()
                .HasIndex(n => new { n.UserId, n.IsRead, n.CreatedAt })
                .HasDatabaseName("IX_Notification_UserId_IsRead_CreatedAt");

            // Configure soft delete global query filters
            modelBuilder.Entity<Post>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Comment>().HasQueryFilter(c => !c.IsDeleted);
        }
    }
}