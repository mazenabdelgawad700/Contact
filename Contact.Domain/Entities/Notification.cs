namespace Contact.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int NotificationType { get; set; }
        public string RelatedEntityId { get; set; }
        public string? ActorUserId { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public virtual User User { get; set; }
        public virtual User? ActorUser { get; set; }
    }
}
