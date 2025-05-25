namespace Contact.Domain.Entities
{
    public class CommentReaction
    {
        public int Id { get; set; }
        public int CommentId { get; set; }
        public string UserId { get; set; }
        public int ReactionType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public virtual Comment Comment { get; set; }
        public virtual User User { get; set; }
    }
}
