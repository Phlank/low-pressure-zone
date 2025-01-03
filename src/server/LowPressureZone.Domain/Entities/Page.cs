using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowPressureZone.Domain.Entities;

public sealed class Page : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; 
}
