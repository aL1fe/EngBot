namespace TelegaEngBot.Identity;

public class IdentityServer
{
    private long _id;
    public IdentityServer(long id)
    {
        _id = id;
    }
    public bool CheckAuth()
    {
        // return true; // identity off
        
        if (_id == 450056320 
            || _id == 906180277 //Alenchik
            || _id == 438560103 //Chilikin
            || _id == 1947844639 //Natalia Eng
            || _id == 97497993 //Dima Barabash
            || _id == 301751068 //Alla
            || _id == 743455767 //Andrey Epicflow
            || _id == 614751690 //Alexandra GlassBox
            || _id == 558784871) // Prygun
        {
            return true;
        }
        return false;
    }
}