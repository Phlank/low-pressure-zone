using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.Broadcasts.Disconnect;

public class PostDisconnect(AzuraCastClient client) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/broadcasts/disconnect");
        Roles(RoleNames.Admin, RoleNames.Organizer);
        Description(builder => builder.Produces(204)
                                      .Produces(400));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = await client.PostBroadcastingAction(BroadcastingActionType.Disconnect);
        if (!response.IsSuccess)
            ThrowError($"Error returned from AzuraCast API: {response.Error.ReasonPhrase ?? response.Error.StatusCode.ToString()}");
        
        await Send.NoContentAsync(ct);
    }
}