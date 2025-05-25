namespace Contact.Domain.Entities
{
    public class PostReaction
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public int ReactionType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
    }
}
