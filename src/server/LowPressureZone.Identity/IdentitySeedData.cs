using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowPressureZone.Identity;

public class IdentitySeedData
{
    public required string AdminUsername { get; set; }
    public required string AdminEmail { get; set; }
    public required string AdminPassword { get; set; }
}
