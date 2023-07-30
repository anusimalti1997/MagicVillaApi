using AutoMapper;
using MagicVillaApi.Data;
using MagicVillaApi.Logging;
using MagicVillaApi.Models.Response;
using MagicVillaApi.Models.Villa;
using MagicVillaApi.Models.Villa.Dto;
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
    public class VillaApiController : ControllerBase
    {
        protected ApiResponse _response;
        private readonly IVillaRepository _dbvilla;
        private readonly IMapper _mapper;
        public ILogging _logger { get; }

        public VillaApiController(ILogging logger, IVillaRepository dbvilla, IMapper mapper)
        {
            _logger = logger;
            _dbvilla = dbvilla;
            _mapper=mapper;
            _response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status200OK,Type =typeof(VillaDTO))]
        public async Task<ActionResult<ApiResponse>> GetVillas()
        {
            try
            {
                _logger.Log("getting all villas", "");

                IEnumerable<Villa> villaList = await _dbvilla.GetAllAsync();

                _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
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

        [HttpGet("id",Name ="getVilla")]
        //[HttpGet("{id}")]
        //[HttpGet("{id:int}")]
        //[ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse>> GetVilla(int id)
        {
            try
            {
                //return VillaStore.villaList;
                if (id == 0)
                {
                    _logger.Log("get villa error with id " + id, "error");
                    return BadRequest();
                }
                var villa = await _dbvilla.GetAsync(x => x.Id == id);
                //var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);

                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return _response;
                }
                _response.Result = _mapper.Map<VillaDTO>(villa);
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
        public async Task<ActionResult<ApiResponse>> CreateVilla([FromBody] VillaCreateDTO villa)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}

                if (await _dbvilla.GetAsync(u => u.Name.ToLower() == villa.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("CustomError", "villa already exist");
                    _response.ErrorMessages = new List<string> { "villa already exist" };
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }



                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.InternalServerError;
                    return _response;
                }

                Villa model = _mapper.Map<Villa>(villa);

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
                await _dbvilla.CreateAsync(model);
                // await _db.SaveChangesAsync();

                _response.Result = model;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.Created;
                //villa.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
                //VillaStore.villaList.Add(villa);
                return CreatedAtRoute("getVilla", new { id = model.Id }, _response);

            }catch(Exception ex)
            {
                _response.ErrorMessages = new List<string> { ex.Message };
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpDelete("id",Name ="DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }

                var villa = await _dbvilla.GetAsync(u => u.Id == id);

                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return _response;
                }

                //VillaStore.villaList.Remove(villa);
                await _dbvilla.RemoveAsync(villa);
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

        [HttpPut("id",Name ="UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> UpdateVilla(int id,[FromBody] VillaUpdateDTO villaDto)
        {
            try
            {
                if (villaDto == null || id != villaDto.Id)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;

                }

                //var villa = _db.Villas.FirstOrDefault(u=>u.Id == id);

                //if(villa == null)
                //{
                //    return NotFound();
                //}

                Villa model = _mapper.Map<Villa>(villaDto);

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

                await _dbvilla.UpdateAsync(model);
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

        [HttpPatch("id", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>>  UpdatePartialVilla(int id , JsonPatchDocument<VillaUpdateDTO> patchDto)
        {
            try
            {
                if (patchDto == null || id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;

                }

                var villa = await _dbvilla.GetAsync(u => u.Id == id, tracked: false);

                VillaUpdateDTO villaDto = _mapper.Map<VillaUpdateDTO>(villa);

                //VillaUpdateDTO villaDto = new()
                //{
                //    Amenity = villa.Amenity,
                //    Details = villa.Details,
                //    Id = villa.Id,
                //    ImageUrl = villa.ImageUrl,
                //    Name = villa.Name,
                //    Occupancy = villa.Occupancy,
                //    Rate = villa.Rate,
                //    Sqft = villa.Sqft,

                //};

                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return _response;
                }

                patchDto.ApplyTo(villaDto, ModelState);
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                    //return BadRequest(ModelState);
                }

                Villa model = _mapper.Map<Villa>(villaDto);

                //Villa model = new Villa()
                //{
                //    Amenity = villaDto.Amenity,
                //    Details = villaDto.Details,
                //    Id = villaDto.Id,
                //    ImageUrl = villa.ImageUrl,
                //    Name = villaDto.Name,
                //    Occupancy = villaDto.Occupancy,
                //    Rate = villaDto.Rate,
                //    Sqft = villaDto.Sqft,
                //    UpdatedDate = DateTime.Now
                //};

                await _dbvilla.UpdateAsync(model);
                // await _db.SaveChangesAsync();
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                //return _response;
            }
            catch (Exception ex)
            {
                _response.ErrorMessages = new List<string> { ex.Message };
                _response.IsSuccess = false;
            }

            return _response;
        }

    }
}
