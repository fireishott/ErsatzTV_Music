using ErsatzTV.Infrastructure.Data;
using ErsatzTV.Core.Api.ManualCollections;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ErsatzTV.Application.ManualCollections
{
    public class GetManualCollectionByIdHandler : IRequestHandler<GetManualCollectionByIdQuery, ManualCollectionResponseModel?>
    {
        private readonly IDbContextFactory<TvContext> _dbContextFactory;

        public GetManualCollectionByIdHandler(IDbContextFactory<TvContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<ManualCollectionResponseModel?> Handle(GetManualCollectionByIdQuery request, CancellationToken cancellationToken)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var collection = await dbContext.Collections
                .Include(c => c.CollectionItems)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (collection == null)
                return null;

            return new ManualCollectionResponseModel(
                collection.Id,
                collection.Name,
                collection.CollectionItems?.Select(item => new CollectionItemResponseModel(
                    item.MediaItemId,
                    CollectionItemType.OtherVideo,
                    $"Item {item.MediaItemId}",
                    null,
                    null,
                    null,
                    null,
                    null
                )).ToList() ?? new List<CollectionItemResponseModel>()
            );
        }
    }
}
