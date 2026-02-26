using MediatR;
using Microsoft.EntityFrameworkCore;
using ErsatzTV.Infrastructure.Data;
using ErsatzTV.Core.Api.ManualCollections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ErsatzTV.Application.ManualCollections
{
    public class GetAllManualCollectionsQueryHandler : IRequestHandler<GetAllManualCollectionsQuery, List<ManualCollectionResponseModel>>
    {
        private readonly IDbContextFactory<TvContext> _dbContextFactory;

        public GetAllManualCollectionsQueryHandler(IDbContextFactory<TvContext> dbContextFactory)
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

            var result = new List<ManualCollectionResponseModel>();
            
            foreach (var collection in collections)
            {
                var items = new List<CollectionItemResponseModel>();
                
                if (collection.CollectionItems != null)
                {
                    foreach (var item in collection.CollectionItems)
                    {
                        items.Add(new CollectionItemResponseModel(
                            item.MediaItemId,
                            CollectionItemType.OtherVideo,
                            $"Item {item.MediaItemId}",
                            null,
                            null,
                            null,
                            null,
                            null
                        ));
                    }
                }
                
                result.Add(new ManualCollectionResponseModel(
                    collection.Id,
                    collection.Name,
                    items
                ));
            }
            
            return result;
        }
    }

    public class GetManualCollectionByIdQueryHandler : IRequestHandler<GetManualCollectionByIdQuery, ManualCollectionResponseModel?>
    {
        private readonly IDbContextFactory<TvContext> _dbContextFactory;

        public GetManualCollectionByIdQueryHandler(IDbContextFactory<TvContext> dbContextFactory)
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

            var items = new List<CollectionItemResponseModel>();
            
            if (collection.CollectionItems != null)
            {
                foreach (var item in collection.CollectionItems)
                {
                    items.Add(new CollectionItemResponseModel(
                        item.MediaItemId,
                        CollectionItemType.OtherVideo,
                        $"Item {item.MediaItemId}",
                        null,
                        null,
                        null,
                        null,
                        null
                    ));
                }
            }

            return new ManualCollectionResponseModel(
                collection.Id,
                collection.Name,
                items
            );
        }
    }
}
