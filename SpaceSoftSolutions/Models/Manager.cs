using System;
using System.Collections.Generic;

namespace SpaceSoftSolutions.Models;

public partial class Manager
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? GoogleId { get; set; }

    public string? ImagePath { get; set; }

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int? DepartmentId { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
}
