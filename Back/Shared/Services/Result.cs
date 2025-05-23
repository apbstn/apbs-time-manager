using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services;

public class Result
{
    public bool Success { get; set; }
    public IExxception Exception { get; set; }
}
