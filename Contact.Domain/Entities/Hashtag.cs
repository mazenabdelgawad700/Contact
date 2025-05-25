namespace Contact.Domain.Entities
{
    public class Hashtag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UsageCount { get; set; }

        // Navigation Properties
        public virtual ICollection<PostHashtag> PostHashtags { get; set; } = new List<PostHashtag>();
    }
}
