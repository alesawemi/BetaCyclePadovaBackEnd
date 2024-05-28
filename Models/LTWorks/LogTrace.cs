using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BetaCycle_Padova.Models.LTWorks;

public partial class LogTrace
{
    [Key]
    public int ErrorID { get; set; }
    public string? MachineName { get; set; }

    public DateTime? Logged { get; set; }

    public string? Level { get; set; }

    public string? Message { get; set; }

    public string? Logger { get; set; }

    public string? Exception { get; set; }
}
