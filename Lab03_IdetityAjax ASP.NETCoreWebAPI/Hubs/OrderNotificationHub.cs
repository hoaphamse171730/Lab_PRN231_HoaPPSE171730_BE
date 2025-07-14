using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Lab03_IdetityAjax_ASP.NETCoreWebAPI.Hubs
{
    [Authorize] 
    public class OrderNotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.Claims
                            .First(c => c.Type == ClaimTypes.NameIdentifier)
                            .Value;
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            await base.OnConnectedAsync();
        }
    }
}
