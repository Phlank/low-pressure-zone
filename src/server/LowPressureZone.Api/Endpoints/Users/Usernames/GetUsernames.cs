using FastEndpoints;
using LowPressureZone.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Users.Usernames;

public class GetUsernames(IdentityContext identityContext) : EndpointWithoutRequest<IEnumerable<UsernameResponse>>
{
    public override void Configure()
    {
        Get("/users/usernames");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var responses = await identityContext.Users
                                             .AsNoTracking()
                                             .Select(user => new UsernameResponse { Id = user.Id, Username = user.UserName! })
                                             .ToListAsync(ct);
        await SendOkAsync(responses, ct);
    }
}
