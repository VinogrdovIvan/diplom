using System;
using System.Collections.Generic;

namespace WebApplication2.Entities;

public partial class Driver
{
    public int DriverId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public DateOnly HireDate { get; set; }

    public bool? IsAvailable { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
