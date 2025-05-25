namespace Contact.Domain.Entities
{
    public class UserProfile
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Bio { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public bool IsProfilePublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public virtual User User { get; set; }
    }
}
