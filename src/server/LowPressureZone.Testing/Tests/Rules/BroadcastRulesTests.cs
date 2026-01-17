using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.Rules;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Testing.Infrastructure.Factories;
using Microsoft.AspNetCore.Http;
using Shouldly;
using Xunit;

namespace LowPressureZone.Testing.Tests.Rules;

public sealed class BroadcastRulesTests
{
    private static BroadcastRules Rules(IHttpContextAccessor accessor) => new(accessor);

    [Fact]
    public void IsDownloadable_ReturnsTrue_WhenRecordingExists()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var broadcast = new StationStreamerBroadcast()
        {
            Recording = new StationStreamerBroadcastRecording()
        };

        // Act
        var result = Rules(accessor).IsDownloadable(broadcast);
        
        // Assert
        result.ShouldBe(true);
    }

    [Fact]
    public void IsDownloadable_ReturnsFalse_WhenRecordingDoesNotExist()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var broadcast = new StationStreamerBroadcast();

        // Act
        var result = Rules(accessor).IsDownloadable(broadcast);
        
        // Assert
        result.ShouldBe(false);
    }

    [Fact]
    public void IsDeletable_ReturnsTrue_WhenAdmin()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(Guid.NewGuid(), [RoleNames.Admin]);
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var broadcast = new StationStreamerBroadcast();
        
        // Act
        var result = Rules(accessor).IsDeletable(broadcast);
        
        // Assert
        result.ShouldBe(true);
    }

    [Fact]
    public void IsDeletable_ReturnsFalse_WhenNotAdmin()
    {
        // Arrange
        var principal = ClaimsPrincipalFactory.Create(Guid.NewGuid());
        var (_, accessor) = HttpContextFactory.Create(user: principal);
        var broadcast = new StationStreamerBroadcast();
        
        // Act
        var result = Rules(accessor).IsDeletable(broadcast);
        
        // Assert
        result.ShouldBe(false);
    }
    
    [Fact]
    public void IsDeletable_ReturnsFalse_WhenNoUser()
    {
        // Arrange
        var (_, accessor) = HttpContextFactory.Create();
        var broadcast = new StationStreamerBroadcast();
        
        // Act
        var result = Rules(accessor).IsDeletable(broadcast);
        
        // Assert
        result.ShouldBeFalse();
    }
}