using System;
using System.Collections.Generic;

namespace EV_test.Models;

public partial class Patient
{
    public int PatientId { get; set; }

    public string PatientName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string Phone { get; set; } = null!;

    public string? Image { get; set; }

    public bool Emergency { get; set; }

    public virtual ICollection<PatientReport> PatientReports { get; set; } = new List<PatientReport>();
}
