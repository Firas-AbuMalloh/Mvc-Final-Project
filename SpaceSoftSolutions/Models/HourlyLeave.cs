using System;
using System.Collections.Generic;

namespace SpaceSoftSolutions.Models;

public partial class HourlyLeave
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
