using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using YactAPI.Data;
using YactAPI.Models;
using YactAPI.Models.Dto;

namespace YactAPI.Controllers
{
    [Route("api/climbs")]
    [ApiController]
    public class ClimbApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;

        public ClimbApiController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Climb> objList = _db.Climbs.ToList();
                _response.Result = _mapper.Map<IEnumerable<ClimbDto>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Climb obj = _db.Climbs.First(u => u.Id == id);
                _response.Result = _mapper.Map<ClimbDto>(obj); 
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
