using System;
using System.Collections.Generic;

namespace BetaCycle_Padova.Models.Users;

public partial class Credential
{
    public int Id { get; set; }

    public string? Password { get; set; }

    public string? Salt { get; set; }

    public int? IdUser { get; set; }

    public virtual User? IdUserNavigation { get; set; }

    public Credential() 
    {
        IdUser = 0;
        Password = null;
        Salt = null;
        IdUserNavigation = null;
    }

    public Credential(string? password, string? salt) //nuovo controller perché quando faccio HttpPost di nuovo User voglio che nuove Credential associate abbiano UserId corrispondente
    {
        Password = password;
        Salt = salt;
    }

    public Credential( string? password, string? salt, int? idUser)
    {     
        Password = password;
        Salt = salt;
        IdUser = idUser;       
    }
}
