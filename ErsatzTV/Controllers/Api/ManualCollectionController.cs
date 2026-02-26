#nullable enable

using ErsatzTV.Application.ManualCollections;
using ErsatzTV.Core;
using ErsatzTV.Core.Api.ManualCollections;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ErsatzTV.Controllers.Api;

[ApiController]
[Route("api/collections/manual")]
public class ManualCollectionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ManualCollectionController> _logger;

    public ManualCollectionController(IMediator mediator, ILogger<ManualCollectionController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<ActionResult<List<ManualCollectionResponseModel>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllManualCollectionsQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ManualCollectionResponseModel>> GetById(int id)
    {
        var result = await _mediator.Send(new GetManualCollectionByIdQuery(id));
        if (result == null)
            return NotFound();
        
        return Ok(result);
    }

    [HttpPost("new")]
    public async Task<IActionResult> Create([FromBody] CreateManualCollectionRequest request)
    {
        var result = await _mediator.Send(new CreateManualCollectionCommand(request.Name));
        return result.Match<IActionResult>(
            r => Ok(new { Id = r.Id }),
            error => BadRequest(error.ToString()));
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateManualCollectionRequest request)
    {
        if (id != request.Id)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(new UpdateManualCollectionCommand(request.Id, request.Name));
        return result.Match<IActionResult>(
            r => Ok(new { Id = r.Id }),
            error => BadRequest(error.ToString()));
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteManualCollectionCommand(id));
        return result.Match<IActionResult>(
            _ => Ok(),
            error => BadRequest(error.ToString()));
    }

    [HttpPost("{collectionId}/items")]
    public async Task<IActionResult> AddItems(int collectionId, [FromBody] AddItemsToCollectionRequest request)
    {
        if (collectionId != request.CollectionId)
            return BadRequest("Collection ID mismatch");

        var result = await _mediator.Send(new AddItemsToManualCollectionCommand(collectionId, request.Items));
        return result.Match<IActionResult>(
            _ => Ok(new { message = $"Successfully added items to collection {collectionId}" }),
            error => BadRequest(error.ToString()));
    }

    [HttpDelete("{collectionId}/items")]
    public async Task<IActionResult> RemoveItems(int collectionId, [FromBody] RemoveItemsFromCollectionRequest request)
    {
        if (collectionId != request.CollectionId)
            return BadRequest("Collection ID mismatch");

        var result = await _mediator.Send(new RemoveItemsFromManualCollectionCommand(collectionId, request.ItemIds));
        return result.Match<IActionResult>(
            _ => Ok(new { message = $"Successfully removed items from collection {collectionId}" }),
            error => BadRequest(error.ToString()));
    }
}

// Request models for the controller
public class CreateManualCollectionRequest
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateManualCollectionRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
