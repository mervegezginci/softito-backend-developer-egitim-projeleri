using System;
using System.Collections.Generic;

namespace emlak_dbfirstmvcproje.Models;

public partial class Property
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string Address { get; set; } = null!;

    public int SquareMeter { get; set; }

    public string RoomCount { get; set; } = null!;

    public int BathCount { get; set; }

    public string? ImageUrl { get; set; }

    public int CityId { get; set; }

    public int PropertyTypeId { get; set; }

    public int RealtorId { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual PropertyType PropertyType { get; set; } = null!;

    public virtual Realtor Realtor { get; set; } = null!;
}
