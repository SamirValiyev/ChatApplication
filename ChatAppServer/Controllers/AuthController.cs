using ChatAppServer.Context;
using ChatAppServer.Dtos;
using ChatAppServer.Models;
using GenericFileService.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController(AppDbContext context) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto request,CancellationToken cancellationToken)
        {
            bool isNameExists=await context.Users.AnyAsync(x=>x.Name==request.name);
            if (!isNameExists)
                return BadRequest(new { Message = "The user name has already been used" });
            string avatar = FileService.FileSaveToServer(request.file, "wwwroot/Avatar/");
            User user = new()
            {
                Avatar = avatar,
                Name = request.name,
            };
            await context.Users.AddAsync(user,cancellationToken);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string name,CancellationToken cancellationToken)
        {
            User? user = await context.Users.FirstOrDefaultAsync(x=>x.Name== name,cancellationToken);
            if (user is null)
                return BadRequest(new { Message = "User not found" });
            user.Status = "online";
            await context.SaveChangesAsync();
            return Ok(user);
        }
    }
}
