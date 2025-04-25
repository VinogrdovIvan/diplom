using System;
using System.Collections.Generic;

namespace WebApplication2.Entities;

public partial class Car
{
    public int CarId { get; set; }

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int Year { get; set; }

    public string Color { get; set; }

    public string LicensePlate { get; set; } = null!;

    public decimal HourlyRate { get; set; }

    public bool IsAvailable { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();
}
