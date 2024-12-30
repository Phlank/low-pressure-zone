namespace LowPressureZone.Api.Endpoints.User;

public partial class CreateUser
{
    public class Request
    {
        public string Name { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
