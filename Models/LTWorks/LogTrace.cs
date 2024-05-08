using System;
using System.Collections.Generic;

namespace BetaCycle_Padova.Models.LTWorks;

public partial class LogTrace
{
    public string? MachineName { get; set; }

    public DateTime? Logged { get; set; }

    public string? Level { get; set; }

    public string? Message { get; set; }

    public string? Logger { get; set; }

    public string? Exception { get; set; }
}
