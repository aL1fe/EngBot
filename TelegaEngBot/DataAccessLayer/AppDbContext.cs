using Microsoft.EntityFrameworkCore;
using TelegaEngBot.AppConfigurations;
using TelegaEngBot.Models;

namespace TelegaEngBot.DataAccessLayer;

public class AppDbContext: DbContext
{
    public DbSet<AppUser> UserList { get; set; } = null!; 
    public DbSet<Article> CommonVocabulary { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLazyLoadingProxies()
            .UseSqlServer(AppConfig.ConnectionString);
    }
}

//dotnet ef migrations add Init
//dotnet ef database update