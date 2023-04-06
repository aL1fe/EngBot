using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegaEngBot.Identity;

internal static class IdentityServer
{
    internal static bool CheckAuth(long id)
    {
        if (id == 450056320 
            || id == 438560103 //Chilikin
            || id == 1947844639 //Natalia
            || id == 97497993 //Dima
            || id == 301751068 //Alla
            ) //558784871 Prygun
        {
            return true;
        }
        return false;
    }
}