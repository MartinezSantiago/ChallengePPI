using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class AssetType
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
}
