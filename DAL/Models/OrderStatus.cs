using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class OrderStatus
{
    public int Id { get; set; }

    public string StatusDescription { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
