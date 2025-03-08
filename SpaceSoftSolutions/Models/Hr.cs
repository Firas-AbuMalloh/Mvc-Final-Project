using System;
using System.Collections.Generic;

namespace SpaceSoftSolutions.Models;

public partial class Hr
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? GoogleId { get; set; }

    public string? ImagePath { get; set; }

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
}
