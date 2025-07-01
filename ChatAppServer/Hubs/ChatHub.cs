using ChatAppServer.Context;
using ChatAppServer.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatAppServer.Hubs;

public class ChatHub(AppDbContext context):Hub
{
    public static Dictionary<string,Guid> Users=new Dictionary<string,Guid>();
    public async Task Connect(Guid userId)
    {
        Users.Add(Context.ConnectionId, userId);
        User? user= await context.Users.FindAsync(userId);
        if (user is not null)
            user.Status = "Online";
        await context.SaveChangesAsync();
        await Clients.All.SendAsync("Users", user);
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Guid userId;
        Users.TryGetValue(Context.ConnectionId, out userId);
        User? user=await context.Users.FindAsync(userId);
        if (user is not null)
            user.Status = "Offline";
        await context.SaveChangesAsync();
        await Clients.All.SendAsync("Users", user);
    }
}
