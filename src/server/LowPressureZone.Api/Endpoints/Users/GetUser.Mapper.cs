using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users
{
    public class GetUserMapper : Mapper<GetUserRequest, GetUserResponse, IdentityUser>
    {
        public override GetUserResponse FromEntity(IdentityUser user)
        {
            return FromEntity(user, new List<string>());
        }

        public GetUserResponse FromEntity(IdentityUser user, IEnumerable<string> roles)
        {
            return new GetUserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName,
                EmailConfirmed = user.EmailConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                Roles = roles
            };
        }
    }
}