namespace CarRental.Shared.Responses
{
    public class CalculateCostResponse
    {
        public decimal TotalCost { get; set; }
        public string Message { get; set; }
        public DriverResponse Driver { get; set; }
    }
}