using System;

namespace CarRental.Shared.Responses
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        public int? DriverId { get; set; }
        public CarResponse Car { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; }

        public bool CanBeCancelled =>
            Status != "Отменен" &&
            StartDate > DateTime.UtcNow.AddHours(24);
    }
}