using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using YactAPI.Data;
using YactAPI.Models;
using YactAPI.Models.Dto;
using YactAPI.Services;

namespace YactAPI.Controllers
{
    [Route("api/activities")]
    [ApiController]
    public class ActivityApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;
        private IFileStorageService _fileStorage;

        public ActivityApiController(AppDbContext db, 
                                     IMapper mapper,
                                     IFileStorageService fileStorage)
        {
            _db = db;
            _mapper = mapper;
            _fileStorage = fileStorage;
            _response = new ResponseDto();
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Activity> objList = _db.Activities.ToList();
                _response.Result = _mapper.Map<IEnumerable<ActivityDto>>(objList);
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
                Activity obj = _db.Activities.First(u => u.Id == id);
                _response.Result = _mapper.Map<ActivityDto>(obj);
            }
            catch (Exception ex) 
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            try
            {
                Activity? obj = _db.Activities.FirstOrDefault(u => u.Id == id);
                if (obj == null) 
                    return NotFound($"File with id {id} not found in DB");
                if (string.IsNullOrEmpty(obj.Path))
                    return NotFound($"File with id {id} has no path saved in DB");
                if (string.IsNullOrEmpty(obj.ContentType))
                    return NotFound($"File with id {id} has no valid ContentType saved in DB");

                var file = await _fileStorage.GetFile(obj.Path);
                if (file == null)
                    return NotFound("File not found in server");

                return File(file, obj.ContentType, obj.Name);
            }
            catch (Exception ex)
            {
                return NotFound($"Error finding file: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ResponseDto> UploadFile(IFormFile file)
        {
            try
            {
                var fileName = await _fileStorage.SaveFile(file);

                // Save into DB
                Activity activity = new Activity
                {
                    Name = file.FileName,
                    Path = fileName,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    UploadDate = DateTime.Now
                };
                _db.Activities.Add(activity);
                _db.SaveChanges();
                _response.Result = _mapper.Map<ActivityDto>(activity);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}/altitude")]
        public ResponseDto GetActivityAltitudePoints(int id)
        {
            try
            {
                //IEnumerable<ActivityPoint> objList = _db.Activities.First(u => u.Id == id);
                //_response.Result = _mapper.Map<IEnumerable<ActivityPointDto>>(objList);
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
