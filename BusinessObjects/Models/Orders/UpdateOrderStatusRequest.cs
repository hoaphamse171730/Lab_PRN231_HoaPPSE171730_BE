using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.Orders
{
    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = null!;
    }
}
