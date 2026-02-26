using ErsatzTV.Core;
using ErsatzTV.Core.Domain;
using ErsatzTV.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ErsatzTV.Application.ManualCollections
{
    public class CreateManualCollectionHandler : IRequestHandler<CreateManualCollectionCommand, Either<BaseError, CreateManualCollectionResult>>
    {
        private readonly IDbContextFactory<TvContext> _dbContextFactory;
        private readonly ILogger<CreateManualCollectionHandler> _logger;

        public CreateManualCollectionHandler(
            IDbContextFactory<TvContext> dbContextFactory,
            ILogger<CreateManualCollectionHandler> logger)
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
}
