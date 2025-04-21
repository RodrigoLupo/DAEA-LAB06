using System;
using System.Collections.Generic;

namespace LAB06_RodrigoLupo.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Age { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
