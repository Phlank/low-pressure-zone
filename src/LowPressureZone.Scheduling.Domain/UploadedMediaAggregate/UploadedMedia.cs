using System.ComponentModel.DataAnnotations;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Scheduling.Domain.UploadedMediaAggregate.Events;
using LowPressureZone.Scheduling.Domain.UploadedMediaAggregate.Rules;

namespace LowPressureZone.Scheduling.Domain.UploadedMediaAggregate;

public class UploadedMedia : Entity
{
    [MaxLength(2048)]
    public string? UploadedFileName
    {
        get;
        private set => field = value?.Trim();
    }

    [MaxLength(256)]
    public string FileName
    {
        get;
        private set => field = value.Trim();
    } = string.Empty;

    [MaxLength(512)]
    public string FilePath
    {
        get;
        private set => field = value.Trim();
    } = string.Empty;

    public int? AzuraCastMediaId { get; private set; }
    public int? AzuraCastPlaylistId { get; private set; }

    // EF Core constructor
    private UploadedMedia()
    {
    }

    public static DomainResult<UploadedMedia> Create(string fileName, string filePath, string? uploadedFileName = null)
    {
        var media = new UploadedMedia
        {
            FileName = fileName,
            FilePath = filePath,
            UploadedFileName = uploadedFileName
        };
        var result = Rule.ApplyIntoResult(media,
                                          new UploadedMediaNameRequiredRule(media),
                                          new UploadedMediaPathRequiredRule(media),
                                          new FileNameLengthCannotExceed256CharactersRule(media),
                                          new FilePathLengthCannotExceed512CharactersRule(media),
                                          new FilePathFormattedCorrectlyRule(media));
        result = result.WithAdditionalEvents(new UploadedMediaCreated(result.Value.Id));
        return result;
    }

    public DomainResult<NoValue> LinkAzuraCastMedia(int azuraCastMediaId)
    {
        var result = Rule.ApplyIntoEmptyResult(new AzuraCastMediaIdMustBeGreaterThan0Rule(azuraCastMediaId),
                                               new CannotLinkMediaIfAlreadyLinkedRule(this, azuraCastMediaId));
        if (result.IsSuccess
            && AzuraCastMediaId is null)
        {
            AzuraCastMediaId = azuraCastMediaId;
            result = result.WithAdditionalEvents(new UploadedMediaLinkedToMedia(Id));
        }

        return result;
    }
    
    public DomainResult<NoValue> LinkAzuraCastPlaylist(int azuraCastPlaylistId)
    {
        var result = Rule.ApplyIntoEmptyResult(new AzuraCastPlaylistIdMustBeGreaterThan0Rule(azuraCastPlaylistId),
                                               new CannotLinkMediaIfAlreadyLinkedRule(this, azuraCastPlaylistId));
        if (result.IsSuccess
            && AzuraCastPlaylistId is null)
        {
            AzuraCastPlaylistId = azuraCastPlaylistId;
            result = result.WithAdditionalEvents(new UploadedMediaLinkedToPlaylist(Id));
        }

        return result;
    }
}