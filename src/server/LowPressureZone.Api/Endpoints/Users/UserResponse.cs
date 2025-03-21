namespace LowPressureZone.Api.Endpoints.Users;

public class UserResponse
{
    public required Guid Id { get; set; }
    public required string DisplayName { get; set; }
    public required bool IsAdmin { get; set; }
    public required DateTime? RegistrationDate { get; set; }
}
