using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Yact.Application.Commands.Cyclists;
using Yact.Application.Queries.Cyclists;
using Yact.Application.Responses;
using Yact.Domain.Entities.Cyclist;
using Yact.Domain.Exceptions.Cyclist;

namespace Yact.Api.Controllers;

[Route("api/fitness")]
[ApiController]
public class FitnessApiController : ControllerBase
{
    private readonly IMediator _mediator;

    public FitnessApiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("get-by-cyclist/{id}")]
    [ProducesResponseType(typeof(IEnumerable<CyclistFitnessDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<CyclistFitnessDto>>> GetFitnessByCyclist(int id)
    {
        try
        {
            var query = new GetFitnessByCyclistIdQuery(id);
            var fitnessList = await _mediator.Send(query);

            return Ok(fitnessList);
        }
        catch (NoCyclistException)
        {
            return NotFound($"Cyclist with ID {id} not found");
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpGet]
    [Route("get-by-cyclist-latest/{id}")]
    [ProducesResponseType(typeof(CyclistFitnessDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<CyclistFitnessDto>> GetLatestCyclistFitness(int id)
    {
        try
        {
            var query = new GetCyclistLatestFitnessQuery(id);
            var fitness = await _mediator.Send(query);

            return Ok(fitness);
        }
        catch (NoCyclistException)
        {
            return NotFound($"Cyclist with ID {id} not found");
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    [Route("create/{cyclistId}")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<int>> CreateFitnessData([FromBody] CyclistFitnessDto fitness, int cyclistId)
    {
        try
        {
            var command = new CreateFitnessCommand(fitness, cyclistId);
            var newData = await _mediator.Send(command);

            return Ok(newData);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpDelete]
    [Route("delete/{id}")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<int>> DeleteFitnessData(int id)
    {
        try
        {
            var command = new DeleteFitnessCommand(id);
            var deleted = await _mediator.Send(command);
            if (deleted == null)
                return NotFound($"Fitness data with ID {id} not found");

            return Ok(deleted);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
