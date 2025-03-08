using System;
using System.Collections.Generic;

namespace SpaceSoftSolutions.Models;

public partial class Task
{
    public int Id { get; set; }

    public string TaskName { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Status { get; set; }

    public int EmployeeId { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
