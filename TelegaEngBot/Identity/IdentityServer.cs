namespace TelegaEngBot.Identity;

internal static class IdentityServer
{
    internal static bool CheckAuth(long id)
    {
        //return true; // identity off
        
        if (id == 450056320 
            || id == 906180277 //Alenchik
            || id == 438560103 //Chilikin
            || id == 1947844639 //Natalia Eng
            || id == 97497993 //Dima Barabash
            || id == 301751068 //Alla
            || id == 743455767 //Andrey Epicflow
            || id == 614751690 //Alexandra GlassBox
            ) //558784871 Prygun
        {
            return true;
        }
        return false;
    }
}