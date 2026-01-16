using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Testing.Data.EntityFactories;
using LowPressureZone.Testing.Infrastructure.Factories;
using Microsoft.AspNetCore.Http;
using Shouldly;
using Xunit;

namespace LowPressureZone.Testing.Tests.Rules;

public sealed class TimeslotRulesTests
{
    private static TimeslotRules Rules(IHttpContextAccessor accessor) => new(accessor);

    [Fact]
    public void PermissionsMethods_ThrowException_WhenPerformerIsNull()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var timeslot = TimeslotFactory.Create(performer: null);

        // Act & Assert
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsEditAuthorized(timeslot));
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsDeleteAuthorized(timeslot));
    }

    [Fact]
    public void IsEditAuthorized_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var performer = PerformerFactory.Create(userId: Guid.NewGuid());
        var timeslot = TimeslotFactory.Create(performer: performer,
                                              startsAt: DateTimeOffset.UtcNow.TopOfHour(1));

        // Act
        var result = Rules(accessor).IsEditAuthorized(timeslot);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsFalse_WhenTimeslotStartsInPast()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create(userId: userId);
        var timeslot = TimeslotFactory.Create(performer: performer,
                                              startsAt: DateTimeOffset.UtcNow.TopOfHour(-1));

        // Act
        var result = Rules(accessor).IsEditAuthorized(timeslot);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsTrue_WhenUserIsLinked_AndTimeslotNotStarted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create(userId: userId);
        var timeslot = TimeslotFactory.Create(performer: performer,
                                              startsAt: DateTimeOffset.UtcNow.TopOfHour(1));

        // Act
        var result = Rules(accessor).IsEditAuthorized(timeslot);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsFalse_WhenUserIsNotLinked()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create(userId: Guid.NewGuid());
        var timeslot = TimeslotFactory.Create(performer: performer,
                                              startsAt: DateTimeOffset.UtcNow.TopOfHour(1));

        // Act
        var result = Rules(accessor).IsEditAuthorized(timeslot);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var performer = PerformerFactory.Create(userId: Guid.NewGuid());
        var timeslot = TimeslotFactory.Create(performer: performer,
                                              startsAt: DateTimeOffset.UtcNow.TopOfHour(1));

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(timeslot);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsFalse_WhenTimeslotStartsInPast()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create(userId: Guid.NewGuid());
        var timeslot = TimeslotFactory.Create(performer: performer,
                                              startsAt: DateTimeOffset.UtcNow.TopOfHour(-1));

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(timeslot);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsTrue_WhenUserIsAdmin_AndTimeslotNotStarted()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create(userId: Guid.NewGuid());
        var timeslot = TimeslotFactory.Create(performer: performer,
                                              startsAt: DateTimeOffset.UtcNow.TopOfHour(1));

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(timeslot);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsTrue_WhenUserIsLinked_AndTimeslotNotStarted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create(userId: userId);
        var timeslot = TimeslotFactory.Create(performer: performer,
                                              startsAt: DateTimeOffset.UtcNow.TopOfHour(1));

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(timeslot);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsFalse_WhenUserIsNotAdminAndNotLinked()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create(userId: Guid.NewGuid());
        var timeslot = TimeslotFactory.Create(performer: performer,
                                              startsAt: DateTimeOffset.UtcNow.TopOfHour(1));

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(timeslot);

        // Assert
        result.ShouldBeFalse();
    }
}