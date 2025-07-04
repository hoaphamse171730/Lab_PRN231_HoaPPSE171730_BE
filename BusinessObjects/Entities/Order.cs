using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class Order
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public DateTime OrderDate { get; set; }

    public string OrderStatus { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
