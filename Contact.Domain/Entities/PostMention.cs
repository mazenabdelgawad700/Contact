namespace Contact.Domain.Entities
{
    public class PostMention
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string MentionedUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public virtual Post Post { get; set; }
        public virtual User MentionedUser { get; set; }
    }
}
