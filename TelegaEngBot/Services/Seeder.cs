using TelegaEngBot.DataAccessLayer;
using TelegaEngBot.Models;

namespace TelegaEngBot.Services;

public static class Seeder
{
    public static void Seed(AppDbContext context)
    {
        context.Dictionary.AddRange(
                new Word() {Id = Guid.NewGuid(), EngWord  = "Dog", RusWord = "Собака", UrlLink = "non"},
                new Word() {Id = Guid.NewGuid(), EngWord  = "Cat", RusWord = "Кот", UrlLink = "non"},
                new Word() {Id = Guid.NewGuid(), EngWord  = "Mouse", RusWord = "Мышь", UrlLink = "non"}
            );
        context.SaveChanges();
    }
}
