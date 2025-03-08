using System;
using System.Collections.Generic;

namespace SpaceSoftSolutions.Models;
public class EvaluationViewModel
{
    public int EmployeeId { get; set; }
    public int ManagerId { get; set; }

    // الإجابات على الأسئلة
    public string Question1 { get; set; }
    public string Question2 { get; set; }
    public string Question3 { get; set; }
    public string Question4 { get; set; }
    public string Question5 { get; set; }
    public string Question6 { get; set; }
    public string Question7 { get; set; }
    public string Question8 { get; set; }
    public string Question9 { get; set; }
    public string Question10 { get; set; }

    public string Comments { get; set; }
    public DateTime DateEvaluated { get; set; }
}
