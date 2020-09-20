using AutoMapper;
using BandAPI.Entities;
using BandAPI.Helpers;
using BandAPI.Models;
using BandAPI.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace BandAPI.Controllers
{
    [ApiController] // make it an API controller, giving some functionalities to this controller
    [Route("api/bands")] // Create a route for this controller
    public class BandsController : ControllerBase
    {
        private readonly IBandAlbumRepository _bandAlbumRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyValidationService _propertyValidationService;
        private WebSocket _socket;

        public BandsController(IBandAlbumRepository bandAlbumRepository, IMapper mapper, IPropertyMappingService propertyMappingService, IPropertyValidationService propertyValidationService) // there's no reference, how can we add mapper here?
        {
            _bandAlbumRepository = bandAlbumRepository ?? throw new ArgumentNullException(nameof(bandAlbumRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyValidationService = propertyValidationService ?? throw new ArgumentNullException(nameof(propertyValidationService));
        }




        //  Start Creating the API Calls



        [HttpGet(Name = "GetBands")]
        [HttpHead] // return the infomation of the server (infomation in the header) to client
        public IActionResult GetBands([FromQuery] BandResourceParameters bandResourceParameters) // It will automatically map to the query strings?
        {
            if (!_propertyMappingService.ValidMappingExists<BandDto, Band>(bandResourceParameters.OrderBy))
                return BadRequest();

            if (!_propertyValidationService.HasValidProperties<BandDto>(bandResourceParameters.Fields))
                return BadRequest();

            var bandsFromRepo = _bandAlbumRepository.GetBands(bandResourceParameters);

            var previousPagePageLink = bandsFromRepo.HasPrevious ? CreateBandsUri(bandResourceParameters, UriType.PreviousPage) : null;

            var nextPageLink = bandsFromRepo.HasNext ? CreateBandsUri(bandResourceParameters, UriType.NextPage) : null;

            var metaData = new
            {
                totalCount = bandsFromRepo.TotalCount,
                pageSize = bandsFromRepo.PageSize,
                currentPage = bandsFromRepo.CurrentPage,
                totalPages = bandsFromRepo.TotalPages,
                previousPagePageLink,
                nextPageLink,
            };

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData)); // Adding the Pagination Header to the Response

            return Ok(_mapper.Map<IEnumerable<BandDto>>(bandsFromRepo).ShapeData(bandResourceParameters.Fields)); // Serialise the Data to JSON
        }

        [HttpGet("{bandId}", Name = "GetBand")]
        public async Task<IActionResult> GetBand(Guid bandId, [FromQuery] string fields)
        {
            if (!_propertyValidationService.HasValidProperties<BandDto>(fields))
                return BadRequest();

            var bandFromRepo = await _bandAlbumRepository.GetBand(bandId);

            if (bandFromRepo == null)
                return NotFound();

            return Ok(_mapper.Map<BandDto>(bandFromRepo).ShapeData<BandDto>(fields));
        }

        // Data Transform Object (DTO)

        [HttpPost]
        public async Task<ActionResult<BandDto>> CreateNewBand([FromBody] BandForCreatingDto band)
        {
            // returning band

            var bandEntity = _mapper.Map<Band>(band);
            _bandAlbumRepository.AddBand(bandEntity); // After adding the BandEntity will get band ID
            await _bandAlbumRepository.Save();
            var bandToReturn = _mapper.Map<BandDto>(bandEntity);
            return CreatedAtRoute("GetBand", new { bandId = bandToReturn.Id }, bandToReturn);  // The third argument is the body used for returning the 201 
        }

        [HttpOptions]
        public IActionResult GetBandsOptions()
        {
            Response.Headers.Add("Allow", "GET, POST, DELETE, OPTIONS");
            return Ok();
        }

        [HttpDelete("{bandId}")]
        public async Task<ActionResult> DeleteBand(Guid bandId)
        {

            var bandFromRepo = await _bandAlbumRepository.GetBand(bandId);
            if (bandFromRepo == null)
                return NotFound();

            _bandAlbumRepository.DeleteBand(bandFromRepo); // The Children (Albums) will be delted as well
            await _bandAlbumRepository.Save();

            return Ok(_mapper.Map<BandDto>(bandFromRepo));

        }


        private string CreateBandsUri(BandResourceParameters bandResourceParameters, UriType uriType)
        {
            switch (uriType)
            {
                case UriType.PreviousPage:
                    return Url.Link("GetBands", new
                    {
                        pageNumber = bandResourceParameters.PageNumber - 1,
                        pageSize = bandResourceParameters.PageSize,
                        mainGenre = bandResourceParameters.MainGenre,
                        searchQuery = bandResourceParameters.SearchQuery,
                        OrderBy = bandResourceParameters.OrderBy,
                        Fields = bandResourceParameters.Fields
                    });
                case UriType.NextPage:
                    return Url.Link("GetBands", new
                    {
                        pageNumber = bandResourceParameters.PageNumber + 1,
                        pageSize = bandResourceParameters.PageSize,
                        mainGenre = bandResourceParameters.MainGenre,
                        searchQuery = bandResourceParameters.SearchQuery,
                        OrderBy = bandResourceParameters.OrderBy,
                        Fields = bandResourceParameters.Fields
                    });
                default:
                    return Url.Link("GetBands", new
                    {
                        pageNumber = bandResourceParameters.PageNumber,
                        pageSize = bandResourceParameters.PageSize,
                        mainGenre = bandResourceParameters.MainGenre,
                        searchQuery = bandResourceParameters.SearchQuery,
                        OrderBy = bandResourceParameters.OrderBy,
                        Fields = bandResourceParameters.Fields
                    });

            }
        }













    }
}
