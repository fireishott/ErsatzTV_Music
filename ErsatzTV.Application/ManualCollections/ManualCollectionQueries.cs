using MediatR;
using ErsatzTV.Core.Api.ManualCollections;
using System.Collections.Generic;

namespace ErsatzTV.Application.ManualCollections
{
    public record GetAllManualCollectionsQuery() : IRequest<List<ManualCollectionResponseModel>>;
    public record GetManualCollectionByIdQuery(int Id) : IRequest<ManualCollectionResponseModel?>;
}
