namespace CarRental.Shared.Responses
{
    public class CarAvailabilityResponse
    {
        public int CarId { get; set; }
        public bool IsAvailable { get; set; }
        public string Message { get; set; }
    }
}