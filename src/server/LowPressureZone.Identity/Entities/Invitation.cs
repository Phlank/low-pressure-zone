using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowPressureZone.Identity.Entities;

public class Invitation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Email { get; set; }
    public required string NormalizedEmail { get; set; }
    public required bool IsRegistered { get; set; } = false;
    public required DateTime InvitationDate { get; set; } = DateTime.UtcNow;
    public required bool IsCancelled { get; set; } = false;
    public DateTime? CancellationDate { get; set; }
}
