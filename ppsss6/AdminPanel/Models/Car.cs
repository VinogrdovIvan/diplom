namespace AdminPanel.Models
{
    public partial class Car : IItem
    {
        public int CarId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public string LicensePlate { get; set; }
        public decimal HourlyRate { get; set; }
        public bool IsAvailable { get; set; }

        public int Id
        {
            get => CarId;
            set => CarId = value;
        }

        public string Name
        {
            get => $"{Brand} {Model}";
            set { } 
        }
    }
}