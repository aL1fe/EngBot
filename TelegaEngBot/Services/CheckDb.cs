using System.Runtime.Serialization;
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
        Environment.Exit(0);
        // Seeder.Seed(_dbContext);
        // Console.WriteLine("Database was seeded with test values.");
    }

    public void MatchVocabulary()
    {
        // Get common vocabulary hash
        var commonVocabularyGuids = _dbContext.CommonVocabulary
            .Select(x => x.Id)
            .ToArray();
        var commonVocabularyHash = GetHash(commonVocabularyGuids);
        Console.WriteLine("CommonVoc " + PrintHash(commonVocabularyHash));
        
        // Get user vocabulary hash
        foreach (var appUser in _dbContext.UserList)
        {
            if (appUser.TelegramUserId != 450056320) // todo delete this condition
                continue;
            
            var userVocabularyGuids = appUser.UserVocabulary
                .Select(x => x.Article)
                .Select(x=>x.Id)
                .ToArray();
  
            var userVocabularyHash = GetHash(userVocabularyGuids);
            Console.Write(appUser.TelegramUserId + " " + PrintHash(userVocabularyHash));
                
            if (commonVocabularyHash != userVocabularyHash)
            {
                Console.WriteLine(" - not match");
                // ReconcileData();
            }
            else
            {
                Console.WriteLine(" - match");
            }
        }
    }

    private static void ReconcileData()
    {
        
    }

    private static byte[] GetHash(Guid[] guids)
    {
        Array.Sort(guids);

        using var md5 = MD5.Create();
        var concatenatedInputs = string.Join("", guids);
        var inputBytes = Encoding.UTF8.GetBytes(concatenatedInputs);
        var hash = md5.ComputeHash(inputBytes);
        return hash;
    }

    private static ISerializable PrintHash(byte[] hash)
    {
        var sb = new StringBuilder();
        foreach (var t in hash)
        {
            sb.Append(t.ToString("x2"));
        }
        return sb;
    }
    
}