﻿namespace LowPressureZone.Identity.Entities;

public class Invitation<TKey, TUser> where TUser : class
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required TKey UserId { get; set; }
    public virtual TUser? User { get; set; }
    public required DateTime InvitationDate { get; set; } = DateTime.UtcNow;
    public bool IsRegistered { get; set; } = false;
    public DateTime? RegistrationDate { get; set; }
    public bool IsCancelled { get; set; } = false;
    public DateTime? CancellationDate { get; set; }
}
