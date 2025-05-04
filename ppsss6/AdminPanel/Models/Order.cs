namespace AdminPanel.Models
{
    public partial class Order : IItem
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        public int DriverId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; }

        public string CarInfo { get; set; }
        public string DriverInfo { get; set; }
        public string UserInfo { get; set; }
        public string DatesInfo => $"{StartDate:dd.MM.yyyy} - {EndDate:dd.MM.yyyy}";

        public int Id
        {
            get => OrderId;
            set => OrderId = value;
        }

        public string Name
        {
            get => $"Заказ #{OrderId}";
            set { } 
        }
    }
}