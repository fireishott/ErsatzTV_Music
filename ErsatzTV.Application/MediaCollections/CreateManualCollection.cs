using ErsatzTV.Core;
using MediatR;

namespace ErsatzTV.Application.MediaCollections;

public record CreateManualCollection(string Name) : IRequest<Either<BaseError, CreateManualCollectionResult>>;

public record CreateManualCollectionResult(int Id);
