using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yact.Application.Commands.Cyclists;
using Yact.Application.Queries.Cyclists;
using Yact.Application.Responses;

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

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get()
    {
        try
        {
            var query = new GetCyclistsQuery();
            var cyclists = await _mediator.Send(query);
            return Ok(new ResponseDto
            {
                IsSuccess = true,
                Result = cyclists
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDto
            {
                IsSuccess = false,
                Message = ex.Message
            });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResponseDto>> GetById(int id)
    {
        try
        {
            var query = new GetCyclistByIdQuery(id);
            var cyclist = await _mediator.Send(query);

            if (cyclist == null)
            {
                return NotFound(new ResponseDto
                {
                    IsSuccess = false,
                    Message = $"Cyclist with ID {id} not found"
                });
            }

            return Ok(new ResponseDto
            {
                IsSuccess = true,
                Result = cyclist
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDto
            {
                IsSuccess = false,
                Message = ex.Message
            });
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<ResponseDto>> CreateCyclist([FromBody] CreateCyclistCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(new ResponseDto { Result = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDto
            {
                IsSuccess = false,
                Message = ex.Message
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseDto>> DeleteCyclist(int id)
    {
        try
        {
            var command = new DeleteCyclistCommand(id);
            var result = await _mediator.Send(command);
            return Ok(new ResponseDto { Result = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDto
            {
                IsSuccess = false,
                Message = ex.Message
            });
        }
    }

    [HttpPut]
    public async Task<ActionResult<ResponseDto>> Update([FromBody] UpdateCyclistCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(new ResponseDto { Result = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDto
            {
                IsSuccess = false,
                Message = ex.Message
            });
        }

    }
}
