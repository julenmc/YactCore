using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Yact.Api.Requests.Activities;
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
    private readonly ILogger<ActivityApiController> _logger;

    public ActivityApiController(
        IMediator mediator,
        ILogger<ActivityApiController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Route("get-by-id/{id}")]
    [ProducesResponseType(typeof(ActivityResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ActivityResponse>> Get(Guid id)
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
            _logger.LogError($"Error while getting activity by ID: {ex.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Internal error");
        }
    }

    [HttpGet]
    [Route("get-by-cyclist-id/{id}")]
    [ProducesResponseType(typeof(IEnumerable<ActivityResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<ActivityResponse>>> GetByCyclistId(Guid id)
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
            _logger.LogError($"Error while getting activity by cyclist ID: {ex.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Internal error");
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
            _logger.LogError($"Error while uploading activity: {ex.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Internal error");
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
            _logger.LogError($"Error while updating activity: {ex.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Internal error");
        }
    }

    [HttpPut]
    [Route("update/{id}")]
    [ProducesResponseType(typeof(ActivityResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ActivityResponse>> Update(Guid id, [FromBody] UpdateActivityRequest request)
    {
        try
        {
            var command = new UpdateActivityCommand(id, request.Name, request.Description);
            var activity = await _mediator.Send(command);
            return Ok(activity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while getting cyclist by ID: {ex.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Internal error");
        }
    }
}
