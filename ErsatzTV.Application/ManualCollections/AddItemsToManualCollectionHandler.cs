using ErsatzTV.Core;
using ErsatzTV.Core.Domain;
using ErsatzTV.Infrastructure.Data;
using ErsatzTV.Core.Api.ManualCollections;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ErsatzTV.Application.ManualCollections
{
    public class AddItemsToManualCollectionHandler : IRequestHandler<AddItemsToManualCollectionCommand, Either<BaseError, MediatR.Unit>>
    {
        private readonly IDbContextFactory<TvContext> _dbContextFactory;
        private readonly ILogger<AddItemsToManualCollectionHandler> _logger;

        public AddItemsToManualCollectionHandler(
            IDbContextFactory<TvContext> dbContextFactory,
            ILogger<AddItemsToManualCollectionHandler> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async Task<Either<BaseError, MediatR.Unit>> Handle(
            AddItemsToManualCollectionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

                // Check if collection exists
                var collection = await dbContext.Collections
                    .Include(c => c.CollectionItems)
                    .FirstOrDefaultAsync(c => c.Id == request.CollectionId, cancellationToken);

                if (collection == null)
                    return BaseError.New($"Collection {request.CollectionId} not found");

                var successCount = 0;
                var errors = new List<string>();

                foreach (var item in request.Items)
                {
                    try
                    {
                        // Check if item already exists in collection
                        var exists = collection.CollectionItems?.Any(i => 
                            i.MediaItemId == item.MediaItemId) ?? false;

                        if (exists)
                        {
                            errors.Add($"Item {item.Type}:{item.MediaItemId} already exists");
                            continue;
                        }

                        // Verify the media item exists
                        var mediaExists = await MediaItemExists(dbContext, item.Type, item.MediaItemId, cancellationToken);
                        if (!mediaExists)
                        {
                            errors.Add($"Item {item.Type}:{item.MediaItemId} not found");
                            continue;
                        }

                        var collectionItem = new CollectionItem
                        {
                            CollectionId = request.CollectionId,
                            MediaItemId = item.MediaItemId,
                            CustomIndex = collection.CollectionItems?.Count ?? 0
                        };

                        if (collection.CollectionItems == null)
                            collection.CollectionItems = new List<CollectionItem>();

                        collection.CollectionItems.Add(collectionItem);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to add item {Type}:{Id}", item.Type, item.MediaItemId);
                        errors.Add($"{item.Type}:{item.MediaItemId} - {ex.Message}");
                    }
                }

                if (successCount > 0)
                {
                    await dbContext.SaveChangesAsync(cancellationToken);
                }

                _logger.LogInformation("Added {SuccessCount} items to collection {CollectionId}", 
                    successCount, request.CollectionId);

                if (errors.Count > 0)
                    return BaseError.New($"Partial success - Added {successCount} items. Errors: {string.Join(", ", errors)}");

                return MediatR.Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding items to collection {CollectionId}", request.CollectionId);
                return BaseError.New($"Failed to add items: {ex.Message}");
            }
        }

        private static async Task<bool> MediaItemExists(TvContext dbContext, CollectionItemType type, int mediaItemId, CancellationToken cancellationToken)
        {
            return type switch
            {
                CollectionItemType.Movie => await dbContext.Movies.AnyAsync(m => m.Id == mediaItemId, cancellationToken),
                CollectionItemType.Episode => await dbContext.Episodes.AnyAsync(e => e.Id == mediaItemId, cancellationToken),
                CollectionItemType.Artist => await dbContext.Artists.AnyAsync(a => a.Id == mediaItemId, cancellationToken),
                CollectionItemType.Show => await dbContext.Shows.AnyAsync(s => s.Id == mediaItemId, cancellationToken),
                CollectionItemType.Season => await dbContext.Seasons.AnyAsync(s => s.Id == mediaItemId, cancellationToken),
                CollectionItemType.MusicVideo => await dbContext.MusicVideos.AnyAsync(m => m.Id == mediaItemId, cancellationToken),
                CollectionItemType.Song => await dbContext.Songs.AnyAsync(s => s.Id == mediaItemId, cancellationToken),
                CollectionItemType.OtherVideo => await dbContext.OtherVideos.AnyAsync(o => o.Id == mediaItemId, cancellationToken),
                _ => false
            };
        }
    }
}
