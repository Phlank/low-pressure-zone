using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Testing.Data.EntityFactories;
using LowPressureZone.Testing.Infrastructure.Factories;
using Microsoft.AspNetCore.Http;
using Shouldly;
using Xunit;

namespace LowPressureZone.Testing.Tests.Rules;

public sealed class SoundclashRulesTests
{
    private static SoundclashRules Rules(IHttpContextAccessor accessor) => new(accessor);

    [Fact]
    public void PermissionsMethods_ThrowException_WhenScheduleIsNull()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var soundclash = SoundclashFactory.Create(schedule: null);

        // Act & Assert
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsEditAuthorized(soundclash));
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsDeleteAuthorized(soundclash));
    }

    [Fact]
    public void PermissionsMethods_ThrowException_WhenScheduleCommunityIsNull()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var schedule = ScheduleFactory.Create(community: null);
        var soundclash = SoundclashFactory.Create(schedule: schedule);

        // Act & Assert
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsEditAuthorized(soundclash));
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsDeleteAuthorized(soundclash));
    }

    [Fact]
    public void PermissionsMethods_ThrowException_WhenScheduleCommunityRelationshipsIsNull()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(nullRelationships: true);
        var schedule = ScheduleFactory.Create(community: community);
        var soundclash = SoundclashFactory.Create(schedule: schedule);

        // Act & Assert
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsEditAuthorized(soundclash));
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsDeleteAuthorized(soundclash));
    }

    [Fact]
    public void IsEditAuthorized_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var community = CommunityFactory.Create();
        var schedule = ScheduleFactory.Create(community: community);
        var soundclash = SoundclashFactory.Create(schedule: schedule,
                                                  startsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                                  endsAt: DateTimeOffset.UtcNow.TopOfHour(2));

        // Act
        var result = Rules(accessor).IsEditAuthorized(soundclash);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsFalse_WhenSoundclashAlreadyEnded()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create();
        var schedule = ScheduleFactory.Create(community: community);
        var soundclash = SoundclashFactory.Create(schedule: schedule,
                                                  startsAt: DateTimeOffset.UtcNow.TopOfHour(-2),
                                                  endsAt: DateTimeOffset.UtcNow.AddMinutes(-1));

        // Act
        var result = Rules(accessor).IsEditAuthorized(soundclash);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsTrue_WhenUserIsAdmin_AndSoundclashNotEnded()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create();
        var schedule = ScheduleFactory.Create(community: community);
        var soundclash = SoundclashFactory.Create(schedule: schedule,
                                                  startsAt: DateTimeOffset.UtcNow.TopOfHour(-1),
                                                  endsAt: DateTimeOffset.UtcNow.TopOfHour(1));

        // Act
        var result = Rules(accessor).IsEditAuthorized(soundclash);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsTrue_WhenUserIsOrganizerInCommunity_AndSoundclashNotEnded()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);

        var community = CommunityFactory.Create(relationships:
        [
            CommunityRelationshipFactory.Create(userId: userId, isOrganizer: true)
        ]);
        var schedule = ScheduleFactory.Create(community: community);
        var soundclash = SoundclashFactory.Create(schedule: schedule,
                                                  startsAt: DateTimeOffset.UtcNow.TopOfHour(-1),
                                                  endsAt: DateTimeOffset.UtcNow.TopOfHour(1));

        // Act
        var result = Rules(accessor).IsEditAuthorized(soundclash);

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
        var schedule = ScheduleFactory.Create(community: community);
        var soundclash = SoundclashFactory.Create(schedule: schedule,
                                                  startsAt: DateTimeOffset.UtcNow.TopOfHour(-1),
                                                  endsAt: DateTimeOffset.UtcNow.TopOfHour(1));

        // Act
        var result = Rules(accessor).IsEditAuthorized(soundclash);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var community = CommunityFactory.Create();
        var schedule = ScheduleFactory.Create(community: community);
        var soundclash = SoundclashFactory.Create(schedule: schedule,
                                                  startsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                                  endsAt: DateTimeOffset.UtcNow.TopOfHour(2));

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(soundclash);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsFalse_WhenSoundclashAlreadyStarted()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create();
        var schedule = ScheduleFactory.Create(community: community);
        var soundclash = SoundclashFactory.Create(schedule: schedule,
                                                  startsAt: DateTimeOffset.UtcNow.AddMinutes(-1),
                                                  endsAt: DateTimeOffset.UtcNow.TopOfHour(1));

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(soundclash);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsTrue_WhenUserIsAdmin_AndSoundclashNotStarted()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create();
        var schedule = ScheduleFactory.Create(community: community);
        var soundclash = SoundclashFactory.Create(schedule: schedule,
                                                  startsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                                  endsAt: DateTimeOffset.UtcNow.TopOfHour(2));

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(soundclash);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsTrue_WhenUserIsOrganizerInCommunity_AndSoundclashNotStarted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);

        var community = CommunityFactory.Create(relationships:
        [
            CommunityRelationshipFactory.Create(userId: userId, isOrganizer: true)
        ]);
        var schedule = ScheduleFactory.Create(community: community);
        var soundclash = SoundclashFactory.Create(schedule: schedule,
                                                  startsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                                  endsAt: DateTimeOffset.UtcNow.TopOfHour(2));

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(soundclash);

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
        var schedule = ScheduleFactory.Create(community: community);
        var soundclash = SoundclashFactory.Create(schedule: schedule,
                                                  startsAt: DateTimeOffset.UtcNow.TopOfHour(1),
                                                  endsAt: DateTimeOffset.UtcNow.TopOfHour(2));

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(soundclash);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsTrue_WhenEndIsInFuture()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create();
        var schedule = ScheduleFactory.Create(community: community);
        var soundclash = SoundclashFactory.Create(schedule: schedule,
                                                  startsAt: DateTimeOffset.UtcNow.TopOfHour(-1),
                                                  endsAt: DateTimeOffset.UtcNow.TopOfHour(1));

        // Act
        var result = Rules(accessor).IsEditAuthorized(soundclash);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsFalse_WhenStartIsInPast()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create();
        var schedule = ScheduleFactory.Create(community: community);
        var soundclash = SoundclashFactory.Create(schedule: schedule,
                                                  startsAt: DateTimeOffset.UtcNow.TopOfHour(-1),
                                                  endsAt: DateTimeOffset.UtcNow.TopOfHour(1));

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(soundclash);

        // Assert
        result.ShouldBeFalse();
    }
}