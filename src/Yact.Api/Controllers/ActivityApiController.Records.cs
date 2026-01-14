//using Microsoft.AspNetCore.Mvc;
//using System.Net;
//using Yact.Application.ReadModels.Activities;
//using Yact.Application.UseCases.Records.Queries;

//namespace Yact.Api.Controllers;

//public partial class ActivityApiController
//{
//    [HttpGet]
//    [Route("get-by-id/{id}/records/power")]
//    [ProducesResponseType(typeof(TimeSeriesResponseDto<double>), (int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    public async Task<ActionResult<TimeSeriesResponseDto<double>>> GetPowerById(Guid id)
//    {
//        try
//        {
//            var query = new GetPowerByIdQuery(id);
//            var data = await _mediator.Send(query);

//            if (data == null)
//            {
//                return NotFound();
//            }

//            return Ok(data);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
//        }
//    }

//    [HttpGet]
//    [Route("get-by-id/{id}/records/heart-rate")]
//    [ProducesResponseType(typeof(TimeSeriesResponseDto<int>), (int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    public async Task<ActionResult<TimeSeriesResponseDto<int>>> GetHrById(Guid id)
//    {
//        try
//        {
//            var query = new GetHrByIdQuery(id);
//            var data = await _mediator.Send(query);

//            if (data == null)
//            {
//                return NotFound();
//            }

//            return Ok(data);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
//        }
//    }

//    [HttpGet]
//    [Route("get-by-id/{id}/records/cadence")]
//    [ProducesResponseType(typeof(TimeSeriesResponseDto<int>), (int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    public async Task<ActionResult<TimeSeriesResponseDto<int>>> GetCadenceById(Guid id)
//    {
//        try
//        {
//            var query = new GetCadenceByIdQuery(id);
//            var data = await _mediator.Send(query);

//            if (data == null)
//            {
//                return NotFound();
//            }

//            return Ok(data);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
//        }
//    }

//    //[HttpGet("{id}/records/all")]
//    //public async Task<ActionResult<ResponseDto>> GetAllRecordsById (int id) // should return a csv with all the records
//    //{
//    //    throw new NotImplementedException();
//    //}
//}
