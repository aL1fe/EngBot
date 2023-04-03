using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegaEngBot.Identity;

internal static class IdentityServer
{
    internal static bool CheckAuth(long id)
    {
        if (id != 450056320) //438560103 Chilikin, 558784871 Prygun, Dima
        {
            return false;
        }
        return true;
    }
}