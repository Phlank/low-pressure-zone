using LowPressureZone.Api.Rules;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Testing.Data.EntityFactories;
using LowPressureZone.Testing.Infrastructure.Factories;
using Microsoft.AspNetCore.Http;
using Shouldly;
using Xunit;

namespace LowPressureZone.Testing.Tests.Rules;

public sealed class CommunityRelationshipRulesTests
{
    private static CommunityRelationshipRules Rules(IHttpContextAccessor accessor) => new(accessor);

    private static CommunityRelationship DefaultRelationship => CommunityRelationshipFactory.Create();

    [Fact]
    public void IsEditable_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();

        // Act
        var result = Rules(accessor).IsEditable(DefaultRelationship, DefaultRelationship);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsEditable_ReturnsFalse_WhenNoRelationship()
    {
        // Arrange
        var user = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid());
        var (_, accessor) = HttpContextFactory.Create(user: user);

        // Act
        var result = Rules(accessor).IsEditable(DefaultRelationship, null);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsEditable_ReturnsFalse_WhenPerformerToCommunity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = ClaimsPrincipalFactory.Create(userId: userId,
                                                 roles: [RoleNames.Performer, RoleNames.Organizer]);
        var (_, accessor) = HttpContextFactory.Create(user: user);
        var userRelationship = CommunityRelationshipFactory.Create(userId: userId, isPerformer: true);

        // Act
        var result = Rules(accessor).IsEditable(DefaultRelationship, userRelationship);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsEditable_ReturnsTrue_WhenOrganizerToCommunity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = ClaimsPrincipalFactory.Create(userId: userId,
                                                 roles: [RoleNames.Performer, RoleNames.Organizer]);
        var (_, accessor) = HttpContextFactory.Create(user: user);
        var userRelationship = CommunityRelationshipFactory.Create(userId: userId, isOrganizer: true);

        // Act
        var result = Rules(accessor).IsEditable(DefaultRelationship, userRelationship);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsEditable_ReturnsTrue_WhenAdmin()
    {
        // Arrange
        var user = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(),
                                                 roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: user);

        // Act
        var result = Rules(accessor).IsEditable(DefaultRelationship, null);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsEditable_ThrowsError_WhenCommunityIdsDoNotMatch()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var communityRelationship =
            CommunityRelationshipFactory.Create(communityId: Guid.NewGuid(), userId: Guid.NewGuid());
        var userRelationship = CommunityRelationshipFactory.Create(communityId: Guid.NewGuid(), userId: userId);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => Rules(accessor).IsEditable(communityRelationship, 
                                                                                 userRelationship));
    }

    [Fact]
    public void IsEditable_ThrowsError_WhenContextUserIdDoesNotMatchUserRelationshipUserId()
    {
        // Arrange
        var communityId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid());
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var communityRelationship =
            CommunityRelationshipFactory.Create(communityId: communityId, userId: Guid.NewGuid());
        var userRelationship = CommunityRelationshipFactory.Create(communityId: communityId, userId: Guid.NewGuid());

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => Rules(accessor).IsEditable(communityRelationship,
                                                                                 userRelationship));
    }
}