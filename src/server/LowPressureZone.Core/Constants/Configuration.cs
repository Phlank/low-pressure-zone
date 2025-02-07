using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowPressureZone.Core.Constants;

public record Configuration
{
    public const string BaseConfigurationFile = "appsettings.json";
    public const string DevConfigurationFile = "appsettings.Development.json";
    public const string ProdConfigurationFile = "appsettings.Production.json";
}
