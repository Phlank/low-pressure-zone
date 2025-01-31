using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowPressureZone.Domain.Interfaces;

public interface IDateTimeRange
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
