using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs;

public class ManualLogDto
{
    public DateTime Time { get; set; }
    public string Type { get; set; }
    public bool Activ { get; set; } = true;
}