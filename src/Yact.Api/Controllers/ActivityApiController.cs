using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Yact.Application.Responses;
using Yact.Application.UseCases.Activities.Commands;
using Yact.Application.UseCases.Activities.Queries;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Exceptions.Cyclist;

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

    //[HttpGet]
    //public async Task<ActionResult<IEnumerable<ActivityInfoDto>>> Get()
    //{
    //    try
    //    {
    //        var query = new GetActivitiesQuery();
    //        IEnumerable<ActivityInfoDto> activities = await _mediator.Send(query);
    //        return Ok(activities);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
    //    }
    //}

    [HttpGet]
    [Route("get-by-id/{id}")]
    [ProducesResponseType(typeof(ActivityDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ActivityDto>> Get(Guid id)
    {
        try
        {
            var query = new GetActivityByIdQuery(id);
            var activity = await _mediator.Send(query);

            if (activity == null)
            {
                return NotFound();
            }

            return Ok(activity);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpGet]
    [Route("get-by-cyclist-id/{id}")]
    [ProducesResponseType(typeof(IEnumerable<ActivityDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<ActivityDto>>> GetByCyclistId(Guid id)
    {
        try
        {
            var query = new GetActivitiesByCyclisIdQuery(id);
            var activities = await _mediator.Send(query);

            return Ok(activities);
        }
        catch (CyclistNotFoundException)
        {
            return NotFound($"Cyclist with ID {id} doesn't exist");
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    [Route("upload")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<int>> UploadFile(IFormFile file, Guid cyclistId)
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
            return Ok(activityId);
        }
        catch (NoDataException ex)
        {
            return BadRequest($"Activity has no data. Message: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpDelete]
    [Route("delete/{id}")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<int>> DeleteById(Guid id)
    {
        try
        {
            var command = new DeleteActivityByIdCommand(id);

            var activityId = await _mediator.Send(command);
            if (activityId == Guid.Empty)
            {
                return StatusCode(404, $"Activity with ID {id} not found");
            }
            return Ok(activityId);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpPut]
    [Route("update")]
    [ProducesResponseType(typeof(ActivityDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ActivityDto>> Update([FromBody] UpdateActivityCommand command)
    {
        try
        {
            var activity = await _mediator.Send(command);
            return Ok(activity);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
