using System;
using System.Collections.Generic;

namespace emlak_dbfirstmvcproje.Models;

public partial class Realtor
{
    public int Id { get; set; }

    public string NameSurname { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
}
