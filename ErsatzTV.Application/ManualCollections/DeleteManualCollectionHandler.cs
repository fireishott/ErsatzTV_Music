using ErsatzTV.Core;
using ErsatzTV.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ErsatzTV.Application.ManualCollections
{
    public class DeleteManualCollectionHandler : IRequestHandler<DeleteManualCollectionCommand, Either<BaseError, MediatR.Unit>>
    {
        private readonly IDbContextFactory<TvContext> _dbContextFactory;
        private readonly ILogger<DeleteManualCollectionHandler> _logger;

        public DeleteManualCollectionHandler(
            IDbContextFactory<TvContext> dbContextFactory,
            ILogger<DeleteManualCollectionHandler> logger)
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
}
