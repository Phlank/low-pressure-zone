using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowPressureZone.Identity.Entities;

public class Invitation<TUser> where TUser : class
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string UserId { get; set; }
    public virtual TUser? User { get; set; }
    public required DateTime InvitationDate { get; set; } = DateTime.UtcNow;
    public bool IsRegistered { get; set; } = false;
    public DateTime? RegistrationDate { get; set; }
    public bool IsCancelled { get; set; } = false;
    public DateTime? CancellationDate { get; set; }
}
