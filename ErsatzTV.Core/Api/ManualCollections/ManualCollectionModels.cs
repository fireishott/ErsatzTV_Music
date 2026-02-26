#nullable enable

using System.Text.Json.Serialization;

namespace ErsatzTV.Core.Api.ManualCollections;

public record ManualCollectionResponseModel(
    int Id,
    string Name,
    List<CollectionItemResponseModel> Items);

public record CollectionItemResponseModel(
    int Id,
    CollectionItemType Type,
    string Title,
    string? Year,
    string? Poster,
    string? FilePath,
    int? DurationSeconds,
    Dictionary<string, object>? Metadata);

public record CollectionItemRequest(
    CollectionItemType Type,
    int MediaItemId);

public record AddItemsToCollectionRequest(
    int CollectionId,
    List<CollectionItemRequest> Items);

public record RemoveItemsFromCollectionRequest(
    int CollectionId,
    List<int> ItemIds);

public enum CollectionItemType
{
    Movie,
    Episode,
    Artist,
    Show,
    Season,
    MusicVideo,
    Song,
    OtherVideo
}
