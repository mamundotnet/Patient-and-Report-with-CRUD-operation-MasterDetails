using System;
using System.Collections.Generic;

namespace EV_test.Models;

public partial class Report
{
    public int ReportId { get; set; }

    public string ReportName { get; set; } = null!;

    public virtual ICollection<PatientReport> PatientReports { get; set; } = new List<PatientReport>();
}
