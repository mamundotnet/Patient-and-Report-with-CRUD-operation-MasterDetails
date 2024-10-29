using System;
using System.Collections.Generic;

namespace EV_test.Models;

public partial class PatientReport
{
    public int PatientReportId { get; set; }

    public int ReportId { get; set; }

    public int PatientId { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual Report Report { get; set; } = null!;
}
