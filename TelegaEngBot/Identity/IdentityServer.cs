using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegaEngBot.Identity;

internal static class IdentityServer
{
    internal static bool CheckAuth(long id)
    {
        if (id != 450056320) //438560103 Chilikin, 558784871 Prygun, 1947844639 Natalia, 97497993 Dima
        {
            return false;
        }
        return true;
    }
}