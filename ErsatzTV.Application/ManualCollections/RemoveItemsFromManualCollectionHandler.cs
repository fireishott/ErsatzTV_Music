using ErsatzTV.Core;
using ErsatzTV.Core.Domain;
using ErsatzTV.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ErsatzTV.Application.ManualCollections
{
    public class RemoveItemsFromManualCollectionHandler : IRequestHandler<RemoveItemsFromManualCollectionCommand, Either<BaseError, MediatR.Unit>>
    {
        private readonly IDbContextFactory<TvContext> _dbContextFactory;
        private readonly ILogger<RemoveItemsFromManualCollectionHandler> _logger;

        public RemoveItemsFromManualCollectionHandler(
            IDbContextFactory<TvContext> dbContextFactory,
            ILogger<RemoveItemsFromManualCollectionHandler> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async Task<Either<BaseError, MediatR.Unit>> Handle(
            RemoveItemsFromManualCollectionCommand request,
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

                if (collection.CollectionItems == null || collection.CollectionItems.Count == 0)
                    return BaseError.New("No items in collection");

                var itemsToRemove = collection.CollectionItems
                    .Where(i => request.ItemIds.Contains(i.MediaItemId))
                    .ToList();

                if (itemsToRemove.Count == 0)
                    return BaseError.New("No matching items found in collection");

                foreach (var item in itemsToRemove)
                {
                    collection.CollectionItems.Remove(item);
                }

                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Removed {Count} items from collection {CollectionId}", 
                    itemsToRemove.Count, request.CollectionId);

                return MediatR.Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing items from collection {CollectionId}", request.CollectionId);
                return BaseError.New($"Failed to remove items: {ex.Message}");
            }
        }
    }
}
