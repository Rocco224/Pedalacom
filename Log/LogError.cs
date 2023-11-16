using System;
using System.Collections.Generic;

namespace Pedalacom.Log;

public partial class LogError
{
    public int Id { get; set; }

    public short? WebPart { get; set; }

    public string? ErrorMessage { get; set; }

    public short? ErrorCode { get; set; }

    public DateTime? ErrorDate { get; set; }
}
