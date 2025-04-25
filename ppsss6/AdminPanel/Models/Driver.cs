namespace AdminPanel.Models
{
    public partial class Driver : IItem
    {
        public int DriverId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string LicenseNumber { get; set; }
        public bool IsAvailable { get; set; }

        public int Id
        {
            get => DriverId;
            set => DriverId = value;
        }

        public string Name
        {
            get => $"{FirstName} {LastName}";
            set { }
        }
    }
}