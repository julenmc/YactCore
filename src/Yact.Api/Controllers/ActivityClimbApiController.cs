//using MediatR;
//using Microsoft.AspNetCore.Mvc;
//using System.Net;
//using Yact.Application.ReadModels.ActivityClimbs;
//using Yact.Application.UseCases.ActivityClimbs.Queries;

//namespace Yact.Api.Controllers;

//[Route("api/activity-climbs")]
//[ApiController]
//public class ActivityClimbApiController : ControllerBase
//{
//    private readonly IMediator _mediator;

//    public ActivityClimbApiController(IMediator mediator)
//    {
//        _mediator = mediator;
//    }

//    [HttpGet]
//    [Route("get-by-activity-id/{id}")]
//    [ProducesResponseType(typeof(ActivityClimbReadModel), (int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    public async Task<ActionResult<List<ActivityClimbReadModel>>> GetActivityClimbsByActivityId(Guid id)
//    {
//        try
//        {
//            var query = new GetActivityClimbsByActivityIdQuery(id);
//            var climbList = await _mediator.Send(query);

//            if (climbList == null)
//            {
//                return NotFound();
//            }

//            return Ok(climbList);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
//        }

//    }

//    [HttpGet]
//    [Route("get-by-climb-id/{id}")]
//    [ProducesResponseType(typeof(ActivityClimbReadModel), (int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    public async Task<ActionResult<List<ActivityClimbReadModel>>> GetActivityClimbsByClimbId(Guid id)
//    {
//        try
//        {
//            var query = new GetActivityClimbsByClimbIdQuery(id);
//            var climbList = await _mediator.Send(query);

//            if (climbList == null)
//            {
//                return NotFound();
//            }

//            return Ok(climbList);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
//        }

//    }
//}
