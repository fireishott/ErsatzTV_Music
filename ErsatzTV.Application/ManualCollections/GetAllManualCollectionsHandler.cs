using ErsatzTV.Infrastructure.Data;
using ErsatzTV.Core.Api.ManualCollections;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ErsatzTV.Application.ManualCollections
{
    public class GetAllManualCollectionsHandler : IRequestHandler<GetAllManualCollectionsQuery, List<ManualCollectionResponseModel>>
    {
        private readonly IDbContextFactory<TvContext> _dbContextFactory;

        public GetAllManualCollectionsHandler(IDbContextFactory<TvContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<ManualCollectionResponseModel>> Handle(GetAllManualCollectionsQuery request, CancellationToken cancellationToken)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var collections = await dbContext.Collections
                .Include(c => c.CollectionItems)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);

            return collections.Select(collection => new ManualCollectionResponseModel(
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
            )).ToList();
        }
    }
}
