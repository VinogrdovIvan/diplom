namespace WebApplication2.Entities
{
    public class Session
    {
        public int SessionId { get; set; }

        public int UserId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }

        public virtual User? User { get; set; }

    }
}
