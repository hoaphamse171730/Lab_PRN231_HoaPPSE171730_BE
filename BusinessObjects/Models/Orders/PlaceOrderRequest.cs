﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.Orders
{
    public class PlaceOrderRequest
    {
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
