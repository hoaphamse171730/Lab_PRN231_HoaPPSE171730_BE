using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Shared
{
    public static class ApiRoutes
    {
        public static string Orchids => $"{Constants.ApiBaseUrl}/api/Orchids";
        public static string Categories => $"{Constants.ApiBaseUrl}/api/Categories";
        public static string Accounts => $"{Constants.ApiBaseUrl}/api/Accounts";
        public static string Orders => $"{Constants.ApiBaseUrl}/api/Orders";
        public static string OrderDetails => $"{Constants.ApiBaseUrl}/api/OrderDetails";
        public static string Roles => $"{Constants.ApiBaseUrl}/api/Roles";
        public static string AuthRegister => $"{Constants.ApiBaseUrl}/api/Auth/register";
        public static string AuthLogin => $"{Constants.ApiBaseUrl}/api/Auth/login";
        public static string AuthMe => $"{Constants.ApiBaseUrl}/api/Auth/me";
        public static string AuthChangePassword => $"{Constants.ApiBaseUrl}/api/Auth/change-password";
        public static string PlaceOrder => Orders;
        public static string Pay => $"{Orders}/pay";
        public static string VnPayReturn => $"{Orders}/vnpay-return";
        public static string OrderHub => $"{Constants.ApiBaseUrl}/hubs/orders";
        public static string StatsOverview => $"{Constants.ApiBaseUrl}/api/Stats/overview";
        public static string StatsTopOrchids => $"{Constants.ApiBaseUrl}/api/Stats/top-orchids";

    }
}
