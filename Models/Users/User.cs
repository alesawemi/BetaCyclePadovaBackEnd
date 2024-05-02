using System;
using System.Collections.Generic;

namespace BetaCycle_Padova.Models.Users;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Phone { get; set; }

    public string? Mail { get; set; }

    public int OldCustomerId { get; set; }

    public virtual Credential? Credential { get; set; }

    public User() { }

    /// <summary>
    /// User parametrized
    /// </summary>
    /// <param name="name"></param>
    /// <param name="surname"></param>
    /// <param name="phone"></param>
    /// <param name="mail"></param>
    /// <param name="credential"></param>
    public User(string name, string surname, string phone, string mail, Credential credential, int oldCust)
    {       
        Name = name;
        Surname = surname;
        Phone = phone;
        Mail = mail;
        OldCustomerId = oldCust;
        Credential = credential;
    }
}
