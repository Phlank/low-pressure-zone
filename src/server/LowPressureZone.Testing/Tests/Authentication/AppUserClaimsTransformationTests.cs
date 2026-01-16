using System.Security.Claims;
using LowPressureZone.Api.Authentication;
using LowPressureZone.Testing.Infrastructure.Factories;
using LowPressureZone.Testing.Infrastructure.Fixtures;
using Shouldly;
using Xunit;
using TestData = LowPressureZone.Testing.Tests.Authentication.AppUserClaimsTransformationTestsData;

namespace LowPressureZone.Testing.Tests.Authentication;

[Collection("DatabaseQueryTests")]
public sealed class AppUserClaimsTransformationTests(DatabaseFixture databaseFixture)
{
    private AppUserClaimsTransformation Transformation => new(databaseFixture.DataContext);

    [Fact]
    public async Task TransformAsync_DoesNotChangePrincipal_WhenNoNameIdentifierClaim()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create();

        // Act
        await Transformation.TransformAsync(principal);

        // Assert
        principal.Claims.ShouldBeEmpty();
    }

    [Fact]
    public async Task TransformAsync_AddsCheckedClaim_WhenNameIdentifierClaimPresent()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid());

        // Act
        await Transformation.TransformAsync(principal);

        // Assert
        principal.Claims.ShouldContain(claim => claim.Type == "AdditionalClaimsChecked");
    }

    [Fact]
    public async Task TransformAsync_DoesNotAddRoles_WhenCheckedClaimPresent()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: TestData.OrganizerUserId,
                                                      additionalClaimsChecked: true);

        // Act
        await Transformation.TransformAsync(principal);

        // Assert
        principal.Claims.ShouldNotContain(claim => claim.Type == ClaimTypes.Role);
    }

    [Fact]
    public async Task TransformAsync_AddsOrganizerRole_WhenUserIsOrganizer()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: TestData.OrganizerUserId);

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
        var principal = ClaimsPrincipalFactory.Create(userId: TestData.PerformerUserId);

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
        var principal = ClaimsPrincipalFactory.Create(userId: TestData.OrganizerPerformerUserId);

        // Act
        await Transformation.TransformAsync(principal);

        // Assert
        principal.Claims.ShouldContain(claim => claim.Type == ClaimTypes.Role
                                                && claim.Value == "Organizer");
        principal.Claims.ShouldContain(claim => claim.Type == ClaimTypes.Role
                                                && claim.Value == "Performer");
    }
}