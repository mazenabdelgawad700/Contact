namespace Contact.Domain.Entities
{
    public class PostHashtag
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int HashtagId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public virtual Post Post { get; set; }
        public virtual Hashtag Hashtag { get; set; }
    }
}
