using System.ComponentModel;
using FastEndpoints;
using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Testing.Data.RequestFactories;
using LowPressureZone.Testing.Infrastructure.Factories;
using LowPressureZone.Testing.Infrastructure.Fixtures;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace LowPressureZone.Testing.Tests.Endpoints.Communities;

[Collection("Database Query Tests")]
[Trait("Category", "Database")]
public sealed class CommunityRequestValidatorTests
{
    private static readonly CancellationToken Ct = TestContext.Current.CancellationToken;
    private readonly HttpContext _context;
    private readonly CommunityRequestValidator _validator;

    public CommunityRequestValidatorTests(DatabaseFixture databaseFixture)
    {
        (_context, var accessor) = HttpContextFactory.Create();

        _validator = Factory.CreateValidator<CommunityRequestValidator>(ctx =>
        {
            ctx.AddSingleton(accessor);
            ctx.AddSingleton(databaseFixture.DataContext);
        });

        _validator = new CommunityRequestValidator(accessor);
    }

    [Fact]
    public async Task ValidateNewEntity_Valid_ReturnsSuccess()
    {
        // Arrange
        var request = CommunityRequestFactory.Create();

        // Act
        var result = await _validator.ValidateAsync(request, Ct);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateName_Empty_ReturnsFailure()
    {
        // Arrange
        var request = CommunityRequestFactory.Create(" ");

        // Act
        var result = await _validator.ValidateAsync(request, Ct);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(failure => failure.PropertyName == nameof(CommunityRequest.Name));
    }

    [Fact]
    public async Task ValidateName_TooLong_ReturnsFailure()
    {
        // Arrange
        var request = CommunityRequestFactory.Create("a".PadLeft(65, 'a'));

        // Act
        var result = await _validator.ValidateAsync(request, Ct);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(failure => failure.PropertyName == nameof(CommunityRequest.Name));
    }

    [Fact]
    public async Task ValidateName_Duplicate_ReturnsFailure()
    {
        // Arrange
        var request = CommunityRequestFactory.Create(CommunityRequestValidatorTestsData.CommunityName);

        // Act
        var result = await _validator.ValidateAsync(request, Ct);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(failure => failure.PropertyName == nameof(CommunityRequest.Name));
    }

    [Fact]
    public async Task ValidateName_MatchesExistingEntity_WithMatchingId_ReturnsSuccess()
    {
        // Arrange
        _context.Request.RouteValues.Add("id", CommunityRequestValidatorTestsData.CommunityId);
        var request = CommunityRequestFactory.Create(CommunityRequestValidatorTestsData.CommunityName);

        // Act
        var result = await _validator.ValidateAsync(request, Ct);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateUrl_Empty_ReturnsFailure()
    {
        // Arrange
        var request = CommunityRequestFactory.Create(url: " ");

        // Act
        var result = await _validator.ValidateAsync(request, Ct);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(failure => failure.PropertyName == nameof(CommunityRequest.Url));
    }

    [Fact]
    public async Task ValidateUrl_TooLong_ReturnsFailure()
    {
        // Arrange
        var request = CommunityRequestFactory.Create(url: "https://".PadRight(256, 'a') + ".com");

        // Act
        var result = await _validator.ValidateAsync(request, Ct);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(failure => failure.PropertyName == nameof(CommunityRequest.Url));
    }

    [Fact]
    public async ValueTask ValidateUrl_InvalidUrl_ReturnsFailure()
    {
        // Arrange
        var request = CommunityRequestFactory.Create(url: "invalid-url");

        // Act
        var result = await _validator.ValidateAsync(request, Ct);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(failure => failure.PropertyName == nameof(CommunityRequest.Url));
    }
}