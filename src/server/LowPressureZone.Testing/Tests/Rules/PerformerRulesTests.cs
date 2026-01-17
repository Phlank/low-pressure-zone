using LowPressureZone.Api.Rules;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Testing.Data.EntityFactories;
using LowPressureZone.Testing.Infrastructure.Factories;
using Microsoft.AspNetCore.Http;
using Shouldly;
using Xunit;

namespace LowPressureZone.Testing.Tests.Rules;

public sealed class PerformerRulesTests
{
    private static PerformerRules Rules(IHttpContextAccessor accessor) => new(accessor);

    [Fact]
    public void IsTimeslotLinkAuthorized_ReturnsFalse_WhenPerformerIsDeleted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create(userId: userId, isDeleted: true);

        // Act
        var result = Rules(accessor).IsTimeslotLinkAuthorized(performer);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsTimeslotLinkAuthorized_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var performer = PerformerFactory.Create(userId: Guid.NewGuid());

        // Act
        var result = Rules(accessor).IsTimeslotLinkAuthorized(performer);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsTimeslotLinkAuthorized_ReturnsTrue_WhenUserIsLinked()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create(userId: userId);

        // Act
        var result = Rules(accessor).IsTimeslotLinkAuthorized(performer);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsTimeslotLinkAuthorized_ReturnsFalse_WhenUserIsNotLinked()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create(userId: Guid.NewGuid());

        // Act
        var result = Rules(accessor).IsTimeslotLinkAuthorized(performer);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsTimeslotLinkAuthorized_ReturnsFalse_WhenUserIsAdminButNotLinked()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId, roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create(userId: Guid.NewGuid());

        // Act
        var result = Rules(accessor).IsTimeslotLinkAuthorized(performer);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsFalse_WhenPerformerIsDeleted()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create(isDeleted: true);

        // Act
        var result = Rules(accessor).IsEditAuthorized(performer);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var performer = PerformerFactory.Create();

        // Act
        var result = Rules(accessor).IsEditAuthorized(performer);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsTrue_WhenUserIsAdmin()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create();

        // Act
        var result = Rules(accessor).IsEditAuthorized(performer);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsTrue_WhenUserIsLinked()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create(userId: userId);

        // Act
        var result = Rules(accessor).IsEditAuthorized(performer);

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

        // Act
        var result = Rules(accessor).IsEditAuthorized(performer);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsFalse_WhenPerformerIsDeleted()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create(isDeleted: true);

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(performer);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var performer = PerformerFactory.Create();

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(performer);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsTrue_WhenUserIsAdmin()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create();

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(performer);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsTrue_WhenUserIsLinked()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create(userId: userId);

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(performer);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsFalse_WhenUserIsNotLinked()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var performer = PerformerFactory.Create(userId: Guid.NewGuid());

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(performer);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsHiddenFromApi_ReturnsTrue_WhenPerformerIsDeleted()
    {
        // Arrange
        var performer = PerformerFactory.Create(isDeleted: true);

        // Act
        var result = PerformerRules.IsHiddenFromApi(performer);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsHiddenFromApi_ReturnsFalse_WhenPerformerIsNotDeleted()
    {
        // Arrange
        var performer = PerformerFactory.Create(isDeleted: false);

        // Act
        var result = PerformerRules.IsHiddenFromApi(performer);

        // Assert
        result.ShouldBeFalse();
    }
}