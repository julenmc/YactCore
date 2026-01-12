using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Yact.Application.Responses;
using Yact.Application.UseCases.Cyclists.Commands;
using Yact.Application.UseCases.Cyclists.Queries;

namespace Yact.Api.Controllers;

[Route("api/cyclists")]
[ApiController]
public class CyclistApiController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CyclistApiController> _logger;

    public CyclistApiController(IMediator mediator, ILogger<CyclistApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    //[HttpGet]
    //public async Task<ActionResult<IEnumerable<CyclistDto>>> Get()
    //{
    //    try
    //    {
    //        var query = new GetCyclistsQuery();
    //        var cyclists = await _mediator.Send(query);
    //        return Ok(cyclists);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
    //    }
    //}

    [HttpGet]
    [Route("get-by-id/{id}")]
    [ProducesResponseType(typeof(CyclistResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<CyclistResponse>> GetById(Guid id, int? gapDays)
    {
        try
        {
            var query = new GetCyclistByIdQuery(id, gapDays);
            var cyclist = await _mediator.Send(query);

            if (cyclist == null)
            {
                return NotFound();
            }

            return Ok(cyclist);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while getting cyclist by ID: {ex.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Internal error");
        }
    }

    [HttpGet]
    [Route("get-by-last-name/{lastName}")]
    [ProducesResponseType(typeof(CyclistResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<CyclistResponse>>> GetById(string lastName)
    {
        try
        {
            var query = new GetCyclistsByLastNameQuery(lastName);
            var cyclists = await _mediator.Send(query);

            if (cyclists.Count() == 0)
            {
                return NotFound();
            }

            return Ok(cyclists);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while getting cyclist by Last Name: {ex.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Internal error");
        }
    }

    [HttpPost]
    [Route("create")]
    [ProducesResponseType(typeof(CyclistResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CyclistResponse>> CreateCyclist([FromBody] CreateCyclistCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while creating cyclist: {ex.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Internal error");
        }
    }

    [HttpDelete]
    [Route("delete/{id}")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<int>> DeleteCyclist(Guid id)
    {
        try
        {
            var command = new DeleteCyclistCommand(id);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while deleting cyclist: {ex.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Internal error");
        }
    }

    [HttpPut]
    [Route("update-fitness")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<int>> CreateFitnessData([FromBody] UpdateFitnessCommand command)
    {
        try
        {
            var newData = await _mediator.Send(command);

            return Ok(newData);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while updating cyclist fitness: {ex.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Internal error");
        }
    }

    [HttpDelete]
    [Route("delete-fitness/{id}")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<int>> DeleteFitnessData(Guid id)
    {
        try
        {
            var command = new DeleteFitnessCommand(id);
            var deletedId = await _mediator.Send(command);
            if (deletedId == Guid.Empty)
                return NotFound($"Fitness data with ID {id} not found");

            return Ok(deletedId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while deleting cyclist fitness: {ex.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Internal error");
        }
    }
}
