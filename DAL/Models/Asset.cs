using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Asset
{
    public int Id { get; set; }

    public string Ticker { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int AssetTypeId { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual AssetType AssetType { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
