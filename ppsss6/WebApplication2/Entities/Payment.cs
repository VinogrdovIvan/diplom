using System;
using System.Collections.Generic;

namespace WebApplication2.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? OrderId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public virtual Order? Order { get; set; }
}
