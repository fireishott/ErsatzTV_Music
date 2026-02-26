using ErsatzTV.Core;
using MediatR;

namespace ErsatzTV.Application.MediaCollections;

public record UpdateManualCollection(int Id, string Name) : IRequest<Either<BaseError, UpdateManualCollectionResult>>;

public record UpdateManualCollectionResult(int Id);
