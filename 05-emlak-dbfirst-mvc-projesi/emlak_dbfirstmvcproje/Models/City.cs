using System;
using System.Collections.Generic;

namespace emlak_dbfirstmvcproje.Models;

public partial class City
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
}
