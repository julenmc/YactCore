using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yact.Application.DTOs;
using Yact.Application.Handlers.Activities.DeleteActivityById;
using Yact.Application.Handlers.Activities.GetActivities;
using Yact.Application.Handlers.Activities.GetActivitiesById;
using Yact.Application.Handlers.Activities.UpdateActivity;
using Yact.Application.Handlers.Activities.UploadActivity;

namespace Yact.Api.Controllers;

[Route("api/activities")]
[ApiController]
public partial class ActivityApiController : ControllerBase
{
    private readonly IMediator _mediator;

    public ActivityApiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get()
    {
        try
        {
            var query = new GetActivitiesQuery();
            IEnumerable<ActivityInfoDto> activities = await _mediator.Send(query);
            return Ok(new ResponseDto
            {
                IsSuccess = true,
                Result = activities
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
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
        try
        {
            var query = new GetActivityByIdQuery(id);
            var activity = await _mediator.Send(query);

            if (activity == null)
            {
                return NotFound(new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Activity not found"
                });
            }

            return Ok(new ResponseDto
            {
                IsSuccess = true,
                Result = activity
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

    [HttpPost("upload")]
    public async Task<ActionResult<ResponseDto>> UploadFile(IFormFile file, int cyclistId)
    {
        if (file == null)
            return BadRequest("No file uploaded");

        try
        {
            using var stream = file.OpenReadStream();
            var command = new UploadActivityCommand(
                stream,
                file.FileName,
                cyclistId);

            var activityId = await _mediator.Send(command);
            return Ok(new ResponseDto
            {
                IsSuccess = true,
                Result = activityId
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

    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseDto>> DeleteById(int id)
    {
        try
        {
            var command = new DeleteActivityByIdCommand(id);

            var activityId = await _mediator.Send(command);
            if (activityId == -1)
            {
                return StatusCode(500, new ResponseDto
                {
                    IsSuccess = false,
                    Message = $"Activity with ID {id} not found"
                });
            }
            return Ok(new ResponseDto
            {
                IsSuccess = true,
                Result = activityId
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

    [HttpPut]
    public async Task<ActionResult<ResponseDto>> Update([FromBody] ActivityInfoDto activityDto)
    {
        try
        {
            var command = new UpdateActivityCommand(activityDto);

            var activity = await _mediator.Send(command);
            return Ok(new ResponseDto
            {
                IsSuccess = true,
                Result = activity
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
}
