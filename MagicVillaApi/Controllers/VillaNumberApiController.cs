using AutoMapper;
using MagicVillaApi.Data;
using MagicVillaApi.Logging;
using MagicVillaApi.Models.Response;
using MagicVillaApi.Models.Villa;
using MagicVillaApi.Models.Villa.Dto;
using MagicVillaApi.Models.VillaNumber;
using MagicVillaApi.Models.VillaNumber.Dto;
using MagicVillaApi.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace MagicVillaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaNumberApiController : ControllerBase
    {
        protected ApiResponse _response;
        private readonly IVillaNumberRepository _dbvillaNumber;
        private readonly IVillaRepository _dbvilla;
        private readonly IMapper _mapper;
        public ILogging _logger { get; }

        public VillaNumberApiController(ILogging logger, IVillaNumberRepository dbvillaNumber, IMapper mapper
            , IVillaRepository dbvilla)
        {
            _logger = logger;
            _dbvillaNumber = dbvillaNumber;
            _dbvilla = dbvilla;
            _mapper=mapper;
            _response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status200OK,Type =typeof(VillaNumberDto))]
        public async Task<ActionResult<ApiResponse>> GetVillaNumbers()
        {
            try
            {
                _logger.Log("getting all villa Numbers", "");

                IEnumerable<VillaNumber> villaNumberList = await _dbvillaNumber.GetAllAsync();

                _response.Result = _mapper.Map<List<VillaNumberDto>>(villaNumberList);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

             


                //return Ok(VillaStore.villaList);
            }catch(Exception ex)
            {
               
                _response.ErrorMessages=new List<string> { ex.Message};
                _response.IsSuccess = false;
            }

            return _response;
        }

        [HttpGet("id",Name = "getVillaNumber")]
        //[HttpGet("{id}")]
        //[HttpGet("{id:int}")]
        //[ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse>> GetVillaNumber(int id)
        {
            try
            {
                //return VillaStore.villaList;
                if (id == 0)
                {
                    _logger.Log("get villaNumber error with id " + id, "error");
                    return BadRequest();
                }
                var villaNumber = await _dbvillaNumber.GetAsync(x => x.VillaNo == id);
                //var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);

                if (villaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return _response;
                }
                _response.Result = _mapper.Map<VillaNumberDto>(villaNumber);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
              
            }catch( Exception ex )
            {
                _response.ErrorMessages = new List<string> { ex.Message };
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDto villaNumber)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}

                if (await _dbvillaNumber.GetAsync(u => u.VillaNo == villaNumber.VillaNo) != null)
                {
                    ModelState.AddModelError("CustomError", "villaNumber already exist");
                    _response.ErrorMessages = new List<string> { "villaNumber already exist" };
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }
                if(await _dbvilla.GetAsync(x => x.Id == villaNumber.VillaId) == null)
                {
                    ModelState.AddModelError("CustomError", "villaId not exist");
                    _response.ErrorMessages = new List<string> { "villaId not exist" };
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }



                if (villaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.InternalServerError;
                    return _response;
                }

                VillaNumber model = _mapper.Map<VillaNumber>(villaNumber);

                //Villa model = new()
                //{
                //    Amenity=villa.Amenity,
                //    Details=villa.Details,
                //    ImageUrl=villa.ImageUrl,
                //    Name=villa.Name,
                //    Occupancy=villa.Occupancy,
                //    Rate=villa.Rate,
                //    Sqft=villa.Sqft,
                //    CreatedDate=DateTime.Now
                //};
                await _dbvillaNumber.CreateAsync(model);
                // await _db.SaveChangesAsync();

                _response.Result = model;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.Created;
                //villa.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
                //VillaStore.villaList.Add(villa);
                return CreatedAtRoute("getVilla", new { id = model.VillaNo }, _response);

            }catch(Exception ex)
            {
                _response.ErrorMessages = new List<string> { ex.Message };
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpDelete("id",Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> DeleteVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }

                var villaNumber = await _dbvillaNumber.GetAsync(u => u.VillaNo == id);

                if (villaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return _response;
                }

                //VillaStore.villaList.Remove(villa);
                await _dbvillaNumber.RemoveAsync(villaNumber);
                //await _db.SaveChangesAsync();
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                //return _response;
            }catch(Exception ex)
            {
                _response.ErrorMessages = new List<string> { ex.Message };
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPut("id",Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> UpdateVillaNumber(int id,[FromBody] VillaNumberUpdateDto villaNumberDto)
        {
            try
            {
                if (villaNumberDto == null || id != villaNumberDto.VillaNo)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;

                }
                if (await _dbvilla.GetAsync(x => x.Id == villaNumberDto.VillaId) == null)
                {
                    ModelState.AddModelError("CustomError", "villaId is invalid");
                    _response.ErrorMessages = new List<string> { "villaId is invalid" };
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }
                //var villa = _db.Villas.FirstOrDefault(u=>u.Id == id);

                //if(villa == null)
                //{
                //    return NotFound();
                //}

                VillaNumber model = _mapper.Map<VillaNumber>(villaNumberDto);

                //Villa model = new()
                //{

                //    Amenity = villaDto.Amenity,
                //    Details = villaDto.Details,
                //    Id = villaDto.Id,
                //    ImageUrl = villaDto.ImageUrl,
                //    Name = villaDto.Name,
                //    Occupancy = villaDto.Occupancy,
                //    Rate = villaDto.Rate,
                //    Sqft = villaDto.Sqft,
                //    UpdatedDate = DateTime.Now
                //};

                await _dbvillaNumber.UpdateAsync(model);
                //await _db.SaveChangesAsync();

                //villa.Name=villaDto.Name;
                //villa.Sqft=villaDto.Sqft;
                //villa.Occupancy=villaDto.Occupancy;
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                //return _response;
            }catch(Exception ex) 
            {
                _response.ErrorMessages = new List<string> { ex.Message };
                _response.IsSuccess = false;
            }

            return _response;




        }

        //[HttpPatch("id", Name = "UpdatePartialVilla")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<ApiResponse>>  UpdatePartialVilla(int id , JsonPatchDocument<VillaUpdateDTO> patchDto)
        //{
        //    try
        //    {
        //        if (patchDto == null || id == 0)
        //        {
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            return _response;

        //        }

        //        var villa = await _dbvilla.GetAsync(u => u.Id == id, tracked: false);

        //        VillaUpdateDTO villaDto = _mapper.Map<VillaUpdateDTO>(villa);

        //        //VillaUpdateDTO villaDto = new()
        //        //{
        //        //    Amenity = villa.Amenity,
        //        //    Details = villa.Details,
        //        //    Id = villa.Id,
        //        //    ImageUrl = villa.ImageUrl,
        //        //    Name = villa.Name,
        //        //    Occupancy = villa.Occupancy,
        //        //    Rate = villa.Rate,
        //        //    Sqft = villa.Sqft,

        //        //};

        //        if (villa == null)
        //        {
        //            _response.StatusCode = HttpStatusCode.NotFound;
        //            return _response;
        //        }

        //        patchDto.ApplyTo(villaDto, ModelState);
        //        if (!ModelState.IsValid)
        //        {
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            return _response;
        //            //return BadRequest(ModelState);
        //        }

        //        Villa model = _mapper.Map<Villa>(villaDto);

        //        //Villa model = new Villa()
        //        //{
        //        //    Amenity = villaDto.Amenity,
        //        //    Details = villaDto.Details,
        //        //    Id = villaDto.Id,
        //        //    ImageUrl = villa.ImageUrl,
        //        //    Name = villaDto.Name,
        //        //    Occupancy = villaDto.Occupancy,
        //        //    Rate = villaDto.Rate,
        //        //    Sqft = villaDto.Sqft,
        //        //    UpdatedDate = DateTime.Now
        //        //};

        //        await _dbvilla.UpdateAsync(model);
        //        // await _db.SaveChangesAsync();
        //        _response.StatusCode = HttpStatusCode.NoContent;
        //        _response.IsSuccess = true;
        //        //return _response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.ErrorMessages = new List<string> { ex.Message };
        //        _response.IsSuccess = false;
        //    }

        //    return _response;
        //}

    }
}
