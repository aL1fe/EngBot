using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TelegaEngBot.DataAccessLayer;
#pragma warning disable CS8604

namespace TelegaEngBot.Services;

public class CheckDb
{
    private AppDbContext _dbContext;

    public CheckDb(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void CheckDbEmpty()
    {
        if (_dbContext.CommonVocabulary.Any()) return;
        Console.WriteLine("Database is empty.");
        Seeder.Seed(_dbContext);
        Console.WriteLine("Database was seeded with test values.");
    }

    public void MatchVocabulary()
    {
        var userList = _dbContext.UserList
            .Include(x => x.UserVocabulary)!
                .ThenInclude(y => y.Article)
            .Include(x => x.UserSettings);
        
        using var md5 = MD5.Create();
        var guids = _dbContext.CommonVocabulary
            .Select(x => x.Id)
            .ToArray();
        Array.Sort(guids);
            
        var concatenatedInputs = string.Join("", guids);
        var inputBytes = Encoding.UTF8.GetBytes(concatenatedInputs);
        var commonVocabularyHash = md5.ComputeHash(inputBytes);
        
        //
        var sbCom = new StringBuilder();
        foreach (var t in commonVocabularyHash)
        {
            sbCom.Append(t.ToString("x2"));
        }
        Console.WriteLine(sbCom);
        //
        
        foreach (var appUser in userList)
        {
            var userVocabularyGuids = appUser.UserVocabulary
                .Select(x => x.Article)
                .Select(x=>x.Id)
                .ToArray();
                
            Array.Sort(userVocabularyGuids);
            
            var concatenatedUserInputs = string.Join("", userVocabularyGuids);
            var inputUserBytes = Encoding.UTF8.GetBytes(concatenatedUserInputs);
                
            var userVocabularyHash = md5.ComputeHash(inputUserBytes);
                
            // Преобразуем массив байтов в строку
            var sb = new StringBuilder();
            foreach (var t in userVocabularyHash)
            {
                sb.Append(t.ToString("x2"));
            }

            Console.WriteLine(appUser.TelegramUserId + " " + sb);
                
            // if (commonVocabularyHash != userVocabularyHash)
            // {
            //     Console.WriteLine("not match");
            // }
        }
    }
    
}