using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Identity.Entities;

public sealed class Invitation<TKey, TUser> where TUser : class
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required TKey UserId { get; set; }
    public TUser? User { get; set; }
    public required DateTime InvitationDate { get; set; } = DateTime.UtcNow;
    public DateTime LastSentDate { get; set; } = DateTime.UtcNow;
    public bool IsRegistered { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime? CancellationDate { get; set; }

    public static void OnModelCreated(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invitation<TKey, TUser>>().Property(e => e.Id).ValueGeneratedNever();
    }
}