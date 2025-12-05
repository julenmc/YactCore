using Microsoft.AspNetCore.Mvc;
using Yact.Application.DTOs;
using Yact.Application.Handlers.Records.GetPowerById;

namespace Yact.Api.Controllers;

public partial class ActivityApiController
{
    [HttpGet("{id}/records/power")]
    public async Task<ActionResult<ResponseDto>> GetPowerById(int id)
    {
        try
        {
            var query = new GetPowerByIdQuery(id);
            var data = await _mediator.Send(query);

            if (data == null)
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
                Result = data
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

    [HttpGet("{id}/records/heart-rate")]
    public async Task<ActionResult<ResponseDto>> GetHrById(int id)
    {
        try
        {
            var query = new GetHrByIdQuery(id);
            var data = await _mediator.Send(query);

            if (data == null)
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
                Result = data
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

    [HttpGet("{id}/records/cadence")]
    public async Task<ActionResult<ResponseDto>> GetCadenceById(int id)
    {
        try
        {
            var query = new GetCadenceByIdQuery(id);
            var data = await _mediator.Send(query);

            if (data == null)
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
                Result = data
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

    [HttpGet("{id}/records/all")]
    public async Task<ActionResult<ResponseDto>> GetAllRecordsById (int id) // should return a csv with all the records
    {
        throw new NotImplementedException();
    }
}
