using LowPressureZone.Api.Rules;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Testing.Data.EntityFactories;
using LowPressureZone.Testing.Infrastructure.Factories;
using Microsoft.AspNetCore.Http;
using Shouldly;
using Xunit;

namespace LowPressureZone.Testing.Tests.Rules;

public sealed class CommunityRulesTests
{
    private static CommunityRules Rules(IHttpContextAccessor accessor) => new(accessor);

    [Fact]
    public void IsPerformanceAuthorized_ThrowsError_WhenRelationshipsNull()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var community = CommunityFactory.Create(nullRelationships: true);

        // Act & Assert
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsPerformanceAuthorized(community));
    }

    [Fact]
    public void IsPerformanceAuthorized_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var community = CommunityFactory.Create();

        // Act
        var result = Rules(accessor).IsPerformanceAuthorized(community);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsPerformanceAuthorized_ReturnsFalse_WhenCommunityIsDeleted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(isDeleted: true,
                                                relationships:
                                                [
                                                    CommunityRelationshipFactory.Create(userId: userId,
                                                                                        isPerformer: true)
                                                ]);

        // Act
        var result = Rules(accessor).IsPerformanceAuthorized(community);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsPerformanceAuthorized_ReturnsTrue_WhenUserIsAdmin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId, roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create();

        // Act
        var result = Rules(accessor).IsPerformanceAuthorized(community);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsPerformanceAuthorized_ReturnsTrue_WhenUserIsPerformer()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(relationships:
        [
            CommunityRelationshipFactory.Create(userId: userId, isPerformer: true)
        ]);

        // Act
        var result = Rules(accessor).IsPerformanceAuthorized(community);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsPerformanceAuthorized_ReturnsFalse_WhenUserIsNotPerformer()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(relationships:
        [
            CommunityRelationshipFactory.Create(userId: userId, isPerformer: false, isOrganizer: true)
        ]);

        // Act
        var result = Rules(accessor).IsPerformanceAuthorized(community);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsOrganizingAuthorized_ThrowsError_WhenRelationshipsNull()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var community = CommunityFactory.Create(nullRelationships: true);

        // Act & Assert
        Should.Throw<ShouldAssertException>(() => Rules(accessor).IsOrganizingAuthorized(community));
    }

    [Fact]
    public void IsOrganizingAuthorized_ReturnsFalse_WhenCommunityDeleted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(isDeleted: true,
                                                relationships:
                                                [
                                                    CommunityRelationshipFactory.Create(userId: userId,
                                                                                        isOrganizer: true)
                                                ]);
        
        // Act
        var result = Rules(accessor).IsOrganizingAuthorized(community);
        
        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsOrganizingAuthorized_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var community = CommunityFactory.Create();

        // Act
        var result = Rules(accessor).IsOrganizingAuthorized(community);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsOrganizingAuthorized_ReturnsTrue_WhenUserIsOrganizer()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(relationships:
        [
            CommunityRelationshipFactory.Create(userId: userId, isOrganizer: true)
        ]);

        // Act
        var result = Rules(accessor).IsOrganizingAuthorized(community);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsOrganizingAuthorized_ReturnsTrue_WhenUserIsAdmin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId, roles: [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create();

        // Act
        var result = Rules(accessor).IsOrganizingAuthorized(community);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsOrganizingAuthorized_ReturnsFalse_WhenUserIsNotOrganizer()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var principal = ClaimsPrincipalFactory.Create(userId: userId);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(relationships:
        [
            CommunityRelationshipFactory.Create(userId: userId, isOrganizer: false, isPerformer: true)
        ]);

        // Act
        var result = Rules(accessor).IsOrganizingAuthorized(community);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsFalse_WhenCommunityIsDeleted()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(isDeleted: true);
        
        // Act
        var result = Rules(accessor).IsEditAuthorized(community);
        
        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsTrue_WhenUserIsAdmin()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create();

        // Act
        var result = Rules(accessor).IsEditAuthorized(community);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsEditAuthorized_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var community = CommunityFactory.Create();
        
        // Act
        var result = Rules(accessor).IsEditAuthorized(community);
        
        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsFalse_WhenCommunityIsDeleted()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create(isDeleted: true);

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(community);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsTrue_WhenUserIsAdmin()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(userId: Guid.NewGuid(), [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var community = CommunityFactory.Create();

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(community);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsDeleteAuthorized_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var community = CommunityFactory.Create();

        // Act
        var result = Rules(accessor).IsDeleteAuthorized(community);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsHiddenFromApi_ReturnsTrue_WhenCommunityIsDeleted()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var community = CommunityFactory.Create(isDeleted: true);

        // Act
        var result = Rules(accessor).IsHiddenFromApi(community);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsHiddenFromApi_ReturnsFalse_WhenCommunityIsNotDeleted()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var community = CommunityFactory.Create(isDeleted: false);

        // Act
        var result = Rules(accessor).IsHiddenFromApi(community);

        // Assert
        result.ShouldBeFalse();
    }
}