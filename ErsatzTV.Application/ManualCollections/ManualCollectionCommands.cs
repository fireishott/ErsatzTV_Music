using MediatR;
using ErsatzTV.Core;
using ErsatzTV.Core.Api.ManualCollections;
using System.Collections.Generic;

namespace ErsatzTV.Application.ManualCollections
{
    public record CreateManualCollectionCommand(string Name) : IRequest<Either<BaseError, CreateManualCollectionResult>>;
    public record CreateManualCollectionResult(int Id);

    public record UpdateManualCollectionCommand(int Id, string Name) : IRequest<Either<BaseError, UpdateManualCollectionResult>>;
    public record UpdateManualCollectionResult(int Id);

    public record DeleteManualCollectionCommand(int Id) : IRequest<Either<BaseError, MediatR.Unit>>;

    public record AddItemsToManualCollectionCommand(int CollectionId, List<CollectionItemRequest> Items) 
        : IRequest<Either<BaseError, MediatR.Unit>>;

    public record RemoveItemsFromManualCollectionCommand(int CollectionId, List<int> ItemIds) 
        : IRequest<Either<BaseError, MediatR.Unit>>;
}
