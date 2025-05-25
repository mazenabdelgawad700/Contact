using Microsoft.AspNetCore.Identity;

namespace Contact.Domain.Entities
{
    public class User : IdentityUser
    {
        public int FriendsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();
        public virtual ICollection<CommentReaction> CommentReactions { get; set; } = new List<CommentReaction>();
        public virtual ICollection<Friendship> SentFriendRequests { get; set; } = new List<Friendship>();
        public virtual ICollection<Friendship> ReceivedFriendRequests { get; set; } = new List<Friendship>();
        public virtual ICollection<PostMention> PostMentions { get; set; } = new List<PostMention>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
