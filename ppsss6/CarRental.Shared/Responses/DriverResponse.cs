namespace CarRental.Shared.Responses
{
    public class DriverResponse
    {
        public int DriverId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string LicenseNumber { get; set; }
        public bool IsAvailable { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}