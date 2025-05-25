namespace Contact.Domain.Entities
{
    public class PostMedia
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string MediaUrl { get; set; }
        public string MediaType { get; set; }
        public int MediaOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public virtual Post Post { get; set; }
    }
}
