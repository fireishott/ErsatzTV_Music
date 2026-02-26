using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ErsatzTV.Core;
using ErsatzTV.Core.Domain;
using ErsatzTV.Infrastructure.Data;
using ErsatzTV.Core.Api.ManualCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ErsatzTV.Application.ManualCollections
{
    public class CreateManualCollectionCommandHandler : IRequestHandler<CreateManualCollectionCommand, Either<BaseError, CreateManualCollectionResult>>
    {
        private readonly IDbContextFactory<TvContext> _dbContextFactory;
        private readonly ILogger<CreateManualCollectionCommandHandler> _logger;

        public CreateManualCollectionCommandHandler(
            IDbContextFactory<TvContext> dbContextFactory,
            ILogger<CreateManualCollectionCommandHandler> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async Task<Either<BaseError, CreateManualCollectionResult>> Handle(
            CreateManualCollectionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

                // Check if name already exists
                var exists = await dbContext.Collections
                    .AnyAsync(c => c.Name == request.Name, cancellationToken);

                if (exists)
                    return BaseError.New($"Collection with name '{request.Name}' already exists");

                var collection = new Collection
                {
                    Name = request.Name,
                    UseCustomPlaybackOrder = false,
                    CollectionItems = new List<CollectionItem>(),
                    MediaItems = new List<MediaItem>(),
                    MultiCollections = new List<MultiCollection>(),
                    MultiCollectionItems = new List<MultiCollectionItem>()
                };

                await dbContext.Collections.AddAsync(collection, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created manual collection {CollectionId}: {Name}", collection.Id, collection.Name);

                return new CreateManualCollectionResult(collection.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create collection");
                return BaseError.New($"Failed to create collection: {ex.Message}");
            }
        }
    }

    public class UpdateManualCollectionCommandHandler : IRequestHandler<UpdateManualCollectionCommand, Either<BaseError, UpdateManualCollectionResult>>
    {
        private readonly IDbContextFactory<TvContext> _dbContextFactory;
        private readonly ILogger<UpdateManualCollectionCommandHandler> _logger;

        public UpdateManualCollectionCommandHandler(
            IDbContextFactory<TvContext> dbContextFactory,
            ILogger<UpdateManualCollectionCommandHandler> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async Task<Either<BaseError, UpdateManualCollectionResult>> Handle(
            UpdateManualCollectionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

                var collection = await dbContext.Collections
                    .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                if (collection == null)
                    return BaseError.New($"Collection {request.Id} not found");

                // Check if new name already exists (if name changed)
                if (collection.Name != request.Name)
                {
                    var nameExists = await dbContext.Collections
                        .AnyAsync(c => c.Name == request.Name && c.Id != request.Id, cancellationToken);

                    if (nameExists)
                        return BaseError.New($"Collection with name '{request.Name}' already exists");
                }

                collection.Name = request.Name;

                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Updated manual collection {CollectionId}", collection.Id);

                return new UpdateManualCollectionResult(collection.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update collection {CollectionId}", request.Id);
                return BaseError.New($"Failed to update collection: {ex.Message}");
            }
        }
    }

    public class DeleteManualCollectionCommandHandler : IRequestHandler<DeleteManualCollectionCommand, Either<BaseError, MediatR.Unit>>
    {
        private readonly IDbContextFactory<TvContext> _dbContextFactory;
        private readonly ILogger<DeleteManualCollectionCommandHandler> _logger;

        public DeleteManualCollectionCommandHandler(
            IDbContextFactory<TvContext> dbContextFactory,
            ILogger<DeleteManualCollectionCommandHandler> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async Task<Either<BaseError, MediatR.Unit>> Handle(DeleteManualCollectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

                var collection = await dbContext.Collections
                    .Include(c => c.CollectionItems)
                    .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                if (collection == null)
                    return BaseError.New($"Collection {request.Id} not found");

                dbContext.Collections.Remove(collection);
                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Deleted manual collection {CollectionId}", request.Id);

                return MediatR.Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete collection {CollectionId}", request.Id);
                return BaseError.New($"Failed to delete collection: {ex.Message}");
            }
        }
    }

    public class AddItemsToManualCollectionCommandHandler : IRequestHandler<AddItemsToManualCollectionCommand, Either<BaseError, MediatR.Unit>>
    {
        private readonly IDbContextFactory<TvContext> _dbContextFactory;
        private readonly ILogger<AddItemsToManualCollectionCommandHandler> _logger;

        public AddItemsToManualCollectionCommandHandler(
            IDbContextFactory<TvContext> dbContextFactory,
            ILogger<AddItemsToManualCollectionCommandHandler> logger)
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

    public class RemoveItemsFromManualCollectionCommandHandler : IRequestHandler<RemoveItemsFromManualCollectionCommand, Either<BaseError, MediatR.Unit>>
    {
        private readonly IDbContextFactory<TvContext> _dbContextFactory;
        private readonly ILogger<RemoveItemsFromManualCollectionCommandHandler> _logger;

        public RemoveItemsFromManualCollectionCommandHandler(
            IDbContextFactory<TvContext> dbContextFactory,
            ILogger<RemoveItemsFromManualCollectionCommandHandler> logger)
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
