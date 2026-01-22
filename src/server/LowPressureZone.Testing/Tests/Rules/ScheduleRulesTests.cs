using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain.Enums;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Testing.Data.EntityFactories;
using LowPressureZone.Testing.Infrastructure.Factories;
using Microsoft.AspNetCore.Http;
using Shouldly;
using Xunit;

namespace LowPressureZone.Testing.Tests.Rules;

public sealed class ScheduleRulesTests
{
    private static ScheduleRules Rules(IHttpContextAccessor accessor) => new(accessor);

    [Fact]
    public void PermissionsMethods_ThrowException_WhenCommunityIsNull()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var schedule = ScheduleFactory.Create(type: ScheduleType.Hourly,
                                              endsAt: DateTimeOffset.UtcNow.TopOfHour(1));
        
        // Act & Assert
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsAddingTimeslotsAllowed(schedule));
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsAddingSoundclashesAllowed(schedule));
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsEditAuthorized(schedule));
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsDeleteAuthorized(schedule));
    }
    
    [Fact]
    public void PermissionsMethods_ThrowException_WhenCommunityRelationshipsIsNull()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(nullRelationships: true);
        var schedule = ScheduleFactory.Create(type: ScheduleType.Hourly,
                                              endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: community);

        // Act & Assert
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsAddingTimeslotsAllowed(schedule));
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsAddingSoundclashesAllowed(schedule));
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsEditAuthorized(schedule));
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsDeleteAuthorized(schedule));
    }

    [Fact]
    public void IsAddingTimeslotsAllowed_ReturnsFalse_WhenTypeIsNotHourly()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var schedule = ScheduleFactory.Create(type: ScheduleType.Soundclash,
                                              endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsAddingTimeslotsAllowed(schedule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsAddingTimeslotsAllowed_ReturnsFalse_WhenScheduleIsInPast()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var schedule = ScheduleFactory.Create(type: ScheduleType.Hourly,
                                              endsAt: DateTimeOffset.UtcNow.TopOfHour(-1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsAddingTimeslotsAllowed(schedule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsAddingTimeslotsAllowed_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var schedule = ScheduleFactory.Create(type: ScheduleType.Hourly,
                                              endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsAddingTimeslotsAllowed(schedule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsAddingTimeslotsAllowed_ReturnsTrue_WhenUserIsAdmin()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var schedule = ScheduleFactory.Create(type: ScheduleType.Hourly,
                                              endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsAddingTimeslotsAllowed(schedule);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsAddingTimeslotsAllowed_ReturnsTrue_WhenUserIsPerformerInCommunity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(relationships:
        [
            CommunityRelationshipFactory.Create(userId: userId, isPerformer: true)
        ]);
        var schedule = ScheduleFactory.Create(type: ScheduleType.Hourly,
                                              endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: community);

        // Act
        var result = Rules(accessor).IsAddingTimeslotsAllowed(schedule);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsAddingTimeslotsAllowed_ReturnsFalse_WhenUserIsNotPerformerInCommunity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(relationships:
        [
            CommunityRelationshipFactory.Create(userId: userId, isPerformer: false, isOrganizer: true)
        ]);
        var schedule = ScheduleFactory.Create(type: ScheduleType.Hourly,
                                              endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: community);

        // Act
        var result = Rules(accessor).IsAddingTimeslotsAllowed(schedule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsAddingSoundclashesAllowed_ReturnsFalse_WhenTypeIsNotSoundclash()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var schedule = ScheduleFactory.Create(type: ScheduleType.Hourly,
                                              endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsAddingSoundclashesAllowed(schedule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsAddingSoundclashesAllowed_ReturnsFalse_WhenScheduleIsInPast()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var schedule = ScheduleFactory.Create(type: ScheduleType.Soundclash,
                                              endsAt: DateTimeOffset.UtcNow.TopOfHour(-1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsAddingSoundclashesAllowed(schedule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsAddingSoundclashesAllowed_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var schedule = ScheduleFactory.Create(type: ScheduleType.Soundclash,
                                              endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsAddingSoundclashesAllowed(schedule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsAddingSoundclashesAllowed_ReturnsTrue_WhenUserIsAdmin()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var schedule = ScheduleFactory.Create(type: ScheduleType.Soundclash,
                                              endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsAddingSoundclashesAllowed(schedule);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsAddingSoundclashesAllowed_ReturnsTrue_WhenUserIsOrganizerInCommunity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(relationships:
        [
            CommunityRelationshipFactory.Create(userId: userId, isOrganizer: true)
        ]);
        var schedule = ScheduleFactory.Create(type: ScheduleType.Soundclash,
                                              endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: community);

        // Act
        var result = Rules(accessor).IsAddingSoundclashesAllowed(schedule);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsAddingSoundclashesAllowed_ReturnsFalse_WhenUserIsNotOrganizerInCommunity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(relationships:
        [
            CommunityRelationshipFactory.Create(userId: userId, isOrganizer: false, isPerformer: true)
        ]);
        var schedule = ScheduleFactory.Create(type: ScheduleType.Soundclash,
                                              endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: community);

        // Act
        var result = Rules(accessor).IsAddingSoundclashesAllowed(schedule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var schedule = ScheduleFactory.Create(endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsEditAuthorized(schedule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsFalse_WhenScheduleIsInPast()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var schedule = ScheduleFactory.Create(endsAt: DateTimeOffset.UtcNow.TopOfHour(-1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsEditAuthorized(schedule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsTrue_WhenUserIsAdmin()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var schedule = ScheduleFactory.Create(endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsEditAuthorized(schedule);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsTrue_WhenUserIsOrganizerInCommunity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(relationships:
        [
            CommunityRelationshipFactory.Create(userId: userId, isOrganizer: true)
        ]);
        var schedule = ScheduleFactory.Create(endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: community);

        // Act
        var result = Rules(accessor).IsEditAuthorized(schedule);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsFalse_WhenUserIsNotOrganizerInCommunity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(relationships:
        [
            CommunityRelationshipFactory.Create(userId: userId, isOrganizer: false, isPerformer: true)
        ]);
        var schedule = ScheduleFactory.Create(endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: community);

        // Act
        var result = Rules(accessor).IsEditAuthorized(schedule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var schedule = ScheduleFactory.Create(endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(schedule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsFalse_WhenScheduleIsInPast()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var schedule = ScheduleFactory.Create(endsAt: DateTimeOffset.UtcNow.TopOfHour(-1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(schedule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsTrue_WhenUserIsAdmin()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var schedule = ScheduleFactory.Create(endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(schedule);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsTrue_WhenUserIsOrganizerInCommunity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(relationships:
        [
            CommunityRelationshipFactory.Create(userId: userId, isOrganizer: true)
        ]);
        var schedule = ScheduleFactory.Create(endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: community);

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(schedule);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsFalse_WhenUserIsNotOrganizerInCommunity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(relationships:
        [
            CommunityRelationshipFactory.Create(userId: userId, isOrganizer: false, isPerformer: true)
        ]);
        var schedule = ScheduleFactory.Create(endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: community);

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(schedule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsHiddenFromApi_ReturnsTrue_WhenScheduleIsInPast_AndUserIsNotAdminOrOrganizer()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid());
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var schedule = ScheduleFactory.Create(endsAt: DateTimeOffset.UtcNow.TopOfHour(-1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsHiddenFromApi(schedule);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsHiddenFromApi_ReturnsFalse_WhenScheduleIsInPast_AndUserIsAdmin()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var schedule = ScheduleFactory.Create(endsAt: DateTimeOffset.UtcNow.TopOfHour(-1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsHiddenFromApi(schedule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsHiddenFromApi_ReturnsFalse_WhenScheduleIsInPast_AndUserIsOrganizerRole()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Organizer]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var schedule = ScheduleFactory.Create(endsAt: DateTimeOffset.UtcNow.TopOfHour(-1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsHiddenFromApi(schedule);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsHiddenFromApi_ReturnsFalse_WhenScheduleIsNotInPast()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var schedule = ScheduleFactory.Create(endsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                              community: CommunityFactory.Create());

        // Act
        var result = Rules(accessor).IsHiddenFromApi(schedule);

        // Assert
        result.ShouldBeFalse();
    }
}