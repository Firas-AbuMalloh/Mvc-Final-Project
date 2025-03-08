using System;
using System.Collections.Generic;

namespace SpaceSoftSolutions.Models;

public partial class Attendance
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public DateTime PunchIn { get; set; }

    public DateTime? PunchOut { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
