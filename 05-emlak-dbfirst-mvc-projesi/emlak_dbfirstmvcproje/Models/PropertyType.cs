using System;
using System.Collections.Generic;

namespace emlak_dbfirstmvcproje.Models;

public partial class PropertyType
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
}
