namespace CarRental.Shared.Responses
{
    public class ReviewResponse
    {
        public int ReviewId { get; set; }
        public int OrderId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}