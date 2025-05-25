namespace Contact.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public int? ParentCommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
        public virtual Comment ParentComment { get; set; }
        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
        public virtual ICollection<CommentMedia> CommentMedia { get; set; } = new List<CommentMedia>();
        public virtual ICollection<CommentReaction> CommentReactions { get; set; } = new List<CommentReaction>();
    }
}
