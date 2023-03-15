using Microsoft.EntityFrameworkCore;
using TelegaEngBot.AppConfigurations;
using TelegaEngBot.Models;

namespace TelegaEngBot.DataAccessLayer;

public class AppDbContext: DbContext
{
    public DbSet<User> UserList { get; set; } = null!; 
    public DbSet<Word> Dictionary { get; set; } = null!;
    public DbSet<WordWithWeight> UserDictionary { get; set; } = null!;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(AppConfig.ConnectionString);
    }
}

//dotnet ef migrations add Init
//dotnet ef database update