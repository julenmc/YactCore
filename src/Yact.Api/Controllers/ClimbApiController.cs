using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Yact.Api.Requests.Climbs;
using Yact.Application.Responses;
using Yact.Application.UseCases.Climbs.Commands;
using Yact.Application.UseCases.Climbs.Queries;
using Yact.Domain.Exceptions.Climbs;

namespace Yact.Api.Controllers;

[Route("api/climbs")]
[ApiController]
public class ClimbApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private ILogger<ClimbApiController> _logger;

    public ClimbApiController(IMediator mediator, ILogger<ClimbApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Route("get-by-id/{id}")]
    [ProducesResponseType(typeof(ClimbResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ClimbResponse>> GetById(Guid id)
    {
        try
        {
            var query = new GetClimbByIdQuery(id);
            var climb = await _mediator.Send(query);
            if (climb == null)
                return NotFound();

            return Ok(climb);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while getting climb by ID: {ex.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Internal error");
        }
    }

    [HttpGet]
    [Route("get-by-coordinates")]
    [ProducesResponseType(typeof(IEnumerable<ClimbResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<ClimbResponse>>> GetByCoordinates([FromQuery] GetClimbsByCoordinatesQuery query)
    {
        try
        {
            var climbs = await _mediator.Send(query);
            if (climbs.Count() == 0) 
                return NotFound();

            return Ok(climbs);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while getting climbs by coordinates: {ex.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Internal error");
        }
    }

    [HttpPut]
    [Route("update/{climbId}")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<int>> UpdateClimb(Guid climbId, [FromQuery] UpdateClimbRequest request)
    {
        try
        {
            var command = new UpdateClimbCommand(climbId, request.Name);
            var id = await _mediator.Send(command);
            if (id == Guid.Empty)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Climb could not be updated");
            }
            return Ok(id);
        }
        catch (ClimbNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while updating climb: {ex.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Internal error");
        }
    }

    [HttpDelete]
    [Route("delete/{climbId}")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<int>> DeleteClimb(Guid climbId)
    {
        try
        {
            var command = new DeleteClimbByIdCommand(climbId);
            var deleted = await _mediator.Send(command);
            if (deleted == Guid.Empty)
            {
                return NotFound();
            }
            return Ok(deleted);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while deleting climb: {ex.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Internal error");
        }
    }
}
