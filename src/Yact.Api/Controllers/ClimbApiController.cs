using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Yact.Application.Commands.Climbs;
using Yact.Application.Queries.Climbs;
using Yact.Application.Responses;

namespace Yact.Api.Controllers;

[Route("api/climbs")]
[ApiController]
public class ClimbApiController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClimbApiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClimbDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<ClimbDto>>> Get()
    {
        try
        {
            var query = new GetClimbsQuery();
            var climbs = await _mediator.Send(query);
            return Ok(climbs);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpGet]
    [Route("get-by-id/{id}")]
    [ProducesResponseType(typeof(ClimbDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ClimbDto>> GetById(int id)
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
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpGet]
    [Route("get-by-coordinates")]
    [ProducesResponseType(typeof(IEnumerable<ClimbDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<ClimbDto>>> GetByCoordinates([FromQuery] GetClimbsByCoordinatesQuery query)
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
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    [Route("create")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<int>> CreateClimb([FromQuery] CreateClimbCommand command)
    {
        try
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpPut]
    [Route("update")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<int>> UpdateClimb([FromQuery] UpdateClimbCommand command)
    {
        try
        {
            var id = await _mediator.Send(command);
            if (id == -1)
            {
                return NotFound();
            }
            return Ok(id);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpDelete]
    [Route("delete")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<int>> DeleteClimb(int id)
    {
        try
        {
            var command = new DeleteClimbByIdCommand(id);
            var climbId = await _mediator.Send(command);
            if (id == -1)
            {
                return NotFound();
            }
            return Ok(id);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
