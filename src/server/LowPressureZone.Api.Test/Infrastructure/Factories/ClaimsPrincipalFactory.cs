using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LowPressureZone.Api.Test.Infrastructure.Factories;

public class ClaimsPrincipalFactory
{
    public ClaimsPrincipal Create(string? nameIdentifier = null, bool isLoggedIn = false, string[]? roles = null, bool hasClaimsCheckedClaim = false)
    {
        var output = new ClaimsPrincipal();
        var identity = new ClaimsIdentity(authenticationType: isLoggedIn ? "Test" : null);
    }
}
