namespace Contact.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public int PrivacyLevel { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation Properties
        public virtual User User { get; set; }
        public virtual ICollection<PostMedia> PostMedia { get; set; } = new List<PostMedia>();
        public virtual ICollection<PostHashtag> PostHashtags { get; set; } = new List<PostHashtag>();
        public virtual ICollection<PostMention> PostMentions { get; set; } = new List<PostMention>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();
    }
}
