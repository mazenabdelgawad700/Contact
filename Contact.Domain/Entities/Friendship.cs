namespace Contact.Domain.Entities
{
    public class Friendship
    {
        public int Id { get; set; }
        public string RequesterId { get; set; }
        public string ReceiverId { get; set; }
        public int Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? RespondedAt { get; set; }

        // Navigation Properties
        public virtual User Requester { get; set; }
        public virtual User Receiver { get; set; }
    }
}
