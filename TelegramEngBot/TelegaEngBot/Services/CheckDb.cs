using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using NLog;
using TelegaEngBot.DataAccessLayer;
using TelegaEngBot.Models;

namespace TelegaEngBot.Services;

public class CheckDb
{
    private AppDbContext _dbContext;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public CheckDb(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void CheckDbEmpty()
    {
        while (true)
        {
            try
            {
                if (_dbContext.CommonVocabulary.Any()) return;
                Console.WriteLine("Database is empty.");
                Logger.Fatal("Database is empty. Application was closed.");
                Environment.Exit(0);
                // Seeder.Seed(_dbContext);
                // Console.WriteLine("Database was seeded with test values.");
            }
            catch (Exception)
            {
                Console.WriteLine("Database is unavailable.");
                Thread.Sleep(10000);
            }
        }
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
            // if (appUser.TelegramUserId != 450056320) // todo delete this condition
            //     continue;

            // if (!appUser.isVocabSync) // todo add new property to AppUser and add check it in code
            // {
            //     continue;
            // }

            var userVocabularyGuids = appUser.UserVocabulary
                .Select(x => x.Article)
                .Select(x => x.Id)
                .ToArray();

            var userVocabularyHash = GetHash(userVocabularyGuids);
            Console.Write(appUser.TelegramUserId + " " + PrintHash(userVocabularyHash));
            
            if (!commonVocabularyHash.SequenceEqual(userVocabularyHash))
            {
                Console.WriteLine(" - not match");
                SyncVocabularies(appUser);
            }
            else
            {
                Console.WriteLine(" - match");
            }
        }
    }

    private void SyncVocabularies(AppUser appUser)
    {
        // Add new item from CommonVocabulary to UserVocabulary
        foreach (var article in _dbContext.CommonVocabulary)
        {
            if (appUser.UserVocabulary.Any(x => x.Article == article))
                continue;
            appUser.UserVocabulary.Add(new UserVocabularyItem {Article = article, Weight = 10});
            _dbContext.SaveChanges();
        }

        // Delete item from UserVocabulary if they are not in CommonVocabulary
        foreach (var item in appUser.UserVocabulary.ToList())
        {
            if (_dbContext.CommonVocabulary.Contains(item.Article))
                continue;
            appUser.UserVocabulary.Remove(item);
            _dbContext.SaveChanges();
        }
        
        Logger.Info("Common vocabulary and user vocabulary was synchronised for Telegram User Id: " +
                    appUser.TelegramUserId);
        Console.WriteLine("Common vocabulary and user vocabulary was synchronised for Telegram User Id: " +
                          appUser.TelegramUserId);
    }
    
    private void ParallelSyncVocabularies(AppUser appUser)
    {
        Parallel.ForEach(_dbContext.CommonVocabulary, article =>
        {
            if (appUser.UserVocabulary.All(x => x.Article != article))
            {
                appUser.UserVocabulary.Add(new UserVocabularyItem {Article = article, Weight = 10});
            }
        });

        _dbContext.SaveChanges();
    }

    private byte[] GetHash(Guid[] guids)
    {
        Array.Sort(guids);

        using var md5 = MD5.Create();
        var concatenatedInputs = string.Join("", guids);
        var inputBytes = Encoding.UTF8.GetBytes(concatenatedInputs);
        var hash = md5.ComputeHash(inputBytes);
        return hash;
    }

    private ISerializable PrintHash(byte[] hash)
    {
        var sb = new StringBuilder();
        foreach (var t in hash)
        {
            sb.Append(t.ToString("x2"));
        }

        return sb;
    }
}