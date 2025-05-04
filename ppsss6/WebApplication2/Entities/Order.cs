using System;
using System.Collections.Generic;

namespace WebApplication2.Entities;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public int CarId { get; set; }

    public int DriverId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal TotalCost { get; set; }

    public string? Status { get; set; }

    public virtual Car? Car { get; set; }

    public virtual Driver? Driver { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual User? User { get; set; }
}
