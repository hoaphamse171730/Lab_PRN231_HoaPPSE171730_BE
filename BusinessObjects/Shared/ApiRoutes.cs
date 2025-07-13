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
        public static string Accounts => $"{Constants.ApiBaseUrl}/api/Accounts";
        public static string Orders => $"{Constants.ApiBaseUrl}/api/Orders";
        public static string OrderDetails => $"{Constants.ApiBaseUrl}/api/OrderDetails";
        public static string Roles => $"{Constants.ApiBaseUrl}/api/Roles";
        public static string AuthRegister => $"{Constants.ApiBaseUrl}/api/Auth/register";
        public static string AuthLogin => $"{Constants.ApiBaseUrl}/api/Auth/login";
    }
}
