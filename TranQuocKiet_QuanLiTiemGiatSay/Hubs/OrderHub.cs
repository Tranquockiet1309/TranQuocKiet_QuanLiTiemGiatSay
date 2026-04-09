using Microsoft.AspNetCore.SignalR;

namespace TranQuocKiet_QuanLiTiemGiatSay.Hubs
{
    public class OrderHub : Hub
    {
        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }

        public async Task LeaveAdminGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
        }
    }
}
