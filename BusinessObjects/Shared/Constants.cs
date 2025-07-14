using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Shared
{
    public static class Constants
    {
        public const int RoleCustomer = 1;
        public const int RoleStaff = 2;
        public const string ApiBaseUrl = "https://localhost:7244";

        public const string OrderStatusPending = "pending";
        public const string OrderStatusProcessing = "processing";
        public const string OrderStatusCompleted = "completed";
        public const string OrderStatusCancelled = "cancelled";

    }
}
