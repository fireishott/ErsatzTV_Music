using ErsatzTV.Core;
using ErsatzTV.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ErsatzTV.Application.ManualCollections
{
    public class UpdateManualCollectionHandler : IRequestHandler<UpdateManualCollectionCommand, Either<BaseError, UpdateManualCollectionResult>>
    {
        private readonly IDbContextFactory<TvContext> _dbContextFactory;
        private readonly ILogger<UpdateManualCollectionHandler> _logger;

        public UpdateManualCollectionHandler(
            IDbContextFactory<TvContext> dbContextFactory,
            ILogger<UpdateManualCollectionHandler> logger)
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
}
