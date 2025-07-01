using ChatAppServer.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<User> Users { get; set; }

}
