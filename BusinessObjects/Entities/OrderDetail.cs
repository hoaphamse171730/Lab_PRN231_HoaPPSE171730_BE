﻿using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int OrchidId { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public virtual Orchid Orchid { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
