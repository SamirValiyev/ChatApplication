using ChatAppServer.Context;
using ChatAppServer.Dtos;
using ChatAppServer.Hubs;
using ChatAppServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChatsController(AppDbContext context,IHubContext<ChatHub> hubContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetChats(Guid UserId,Guid ToUserId,CancellationToken cancellationToken)
        {
            List<Chat> chats = await context.Chats.Where(x => x.UserId == UserId && x.ToUserId == ToUserId || x.UserId == ToUserId && x.ToUserId == UserId).OrderBy(x => x.Date).ToListAsync(cancellationToken);
            return Ok(chats);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageDto request,CancellationToken cancellationToken)
        {
            Chat chat = new()
            {
                Message = request.message,
                UserId = request.UserId,
                ToUserId = request.ToUserId,
                Date = DateTime.Now
            };
            await context.AddAsync(chat,cancellationToken);  
            await context.SaveChangesAsync(cancellationToken);

            string connectionId = ChatHub.Users.First(x => x.Value == chat.ToUserId).Key;
            await hubContext.Clients.Client(connectionId).SendAsync("Messages", chat);
            return NoContent();
        }
    }
}
