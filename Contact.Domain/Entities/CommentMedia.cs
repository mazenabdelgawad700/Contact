namespace Contact.Domain.Entities
{
    public class CommentMedia
    {
        public int Id { get; set; }
        public int CommentId { get; set; }
        public string MediaUrl { get; set; }
        public string MediaType { get; set; }
        public int MediaOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public virtual Comment Comment { get; set; }
    }
}
