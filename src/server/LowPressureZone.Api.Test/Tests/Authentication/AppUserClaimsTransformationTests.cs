using System.Security.Claims;
using LowPressureZone.Api.Authentication;
using LowPressureZone.Api.Test.Infrastructure.Factories;
using LowPressureZone.Api.Test.Infrastructure.Fixtures;
using Shouldly;
using Xunit;
using Data = LowPressureZone.Api.Test.Tests.AppUserClaimsTransformationTestsData;

namespace LowPressureZone.Api.Test.Tests;

[Collection("DatabaseQueryTests")]
public sealed class AppUserClaimsTransformationTests(DatabaseFixture databaseFixture)
{
    private AppUserClaimsTransformation Transformation => new(databaseFixture.DataContext);

    [Fact]
    public async Task TransformAsync_DoesNotChangePrincipal_WhenNoNameIdentifierClaim()
    {
        // Arrange
        var principal = new ClaimsPrincipalBuilder().Build();

        // Act
        await Transformation.TransformAsync(principal);

        // Assert
        principal.Claims.ShouldBeEmpty();
    }

    [Fact]
    public async Task TransformAsync_AddsCheckedClaim_WhenNameIdentifierClaimPresent()
    {
        // Arrange
        var principal = new ClaimsPrincipalBuilder()
                        .WithUserId(Guid.NewGuid().ToString())
                        .Build();

        // Act
        await Transformation.TransformAsync(principal);

        // Assert
        principal.Claims.ShouldContain(claim => claim.Type == "AdditionalClaimsChecked");
    }

    [Fact]
    public async Task TransformAsync_DoesNotAddRoles_WhenCheckedClaimPresent()
    {
        // Arrange
        var principal = new ClaimsPrincipalBuilder()
                        .WithUserId(Data.OrganizerUserId.ToString())
                        .WithClaimsCheckedClaim()
                        .Build();
        
        // Act
        await Transformation.TransformAsync(principal);
        
        // Assert
        principal.Claims.ShouldNotContain(claim => claim.Type == ClaimTypes.Role);
    }

    [Fact]
    public async Task TransformAsync_AddsOrganizerRole_WhenUserIsOrganizer()
    {
        // Arrange
        var principal = new ClaimsPrincipalBuilder()
                        .WithUserId(Data.OrganizerUserId.ToString())
                        .Build();
        
        // Act
        await Transformation.TransformAsync(principal);
        
        // Assert
        principal.Claims.ShouldContain(claim => claim.Type == ClaimTypes.Role
                                             && claim.Value == "Organizer");
        principal.Claims.ShouldNotContain(claim => claim.Type == ClaimTypes.Role
                                                   && claim.Value == "Performer");
    }
    
    [Fact]
    public async Task TransformAsync_AddsPerformerRole_WhenUserIsPerformer()
    {
        // Arrange
        var principal = new ClaimsPrincipalBuilder()
                        .WithUserId(Data.PerformerUserId.ToString())
                        .Build();
        
        // Act
        await Transformation.TransformAsync(principal);
        
        // Assert
        principal.Claims.ShouldNotContain(claim => claim.Type == ClaimTypes.Role
                                                && claim.Value == "Organizer");
        principal.Claims.ShouldContain(claim => claim.Type == ClaimTypes.Role
                                                   && claim.Value == "Performer");
    }
    
    [Fact]
    public async Task TransformAsync_AddsBothRoles_WhenUserIsInBothRoles()
    {
        // Arrange
        var principal = new ClaimsPrincipalBuilder()
                        .WithUserId(Data.OrganizerPerformerUserId.ToString())
                        .Build();
        
        // Act
        await Transformation.TransformAsync(principal);
        
        // Assert
        principal.Claims.ShouldContain(claim => claim.Type == ClaimTypes.Role
                                                   && claim.Value == "Organizer");
        principal.Claims.ShouldContain(claim => claim.Type == ClaimTypes.Role
                                                && claim.Value == "Performer");
    }
}