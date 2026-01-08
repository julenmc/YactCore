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

    public CyclistApiController(IMediator mediator)
    {
        _mediator = mediator;
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
    [ProducesResponseType(typeof(CyclistDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<CyclistDto>> GetById(Guid id)
    {
        try
        {
            var query = new GetCyclistByIdQuery(id);
            var cyclist = await _mediator.Send(query);

            if (cyclist == null)
            {
                return NotFound();
            }

            return Ok(cyclist);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    [Route("create")]
    [ProducesResponseType(typeof(CyclistDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CyclistDto>> CreateCyclist([FromBody] CreateCyclistCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
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
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpPut]
    [Route("update")]
    [ProducesResponseType(typeof(CyclistDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CyclistDto>> Update([FromBody] UpdateCyclistCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }

    }
}
