using AutoMapper;
using BandAPI.Entities;
using BandAPI.Helpers;
using BandAPI.Models;
using BandAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BandCollectionsController : ControllerBase
    {
        private readonly IBandAlbumRepository _bandAlbumRepository;
        private readonly IMapper _mapper;


        public BandCollectionsController(IBandAlbumRepository bandAlbumRepository, IMapper mapper)
        {
            _bandAlbumRepository = bandAlbumRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<BandDto>>> CreateBandCollection([FromBody] IEnumerable<BandForCreatingDto> bandCollection)
        {
            var bandsEntities = _mapper.Map<IEnumerable<Band>>(bandCollection);

            _bandAlbumRepository.AddBands(bandsEntities);

            await _bandAlbumRepository.Save();

            // Select Method
            // bandsEntities.Select(b => b.Id);
            // Linq 
            // from b in bandsEntities select b.Id

            var bandsToReturn = _mapper.Map<IEnumerable<BandDto>>(bandsEntities);

            return CreatedAtRoute("GetBandsByIds", new { bandIds = from b in bandsToReturn select b.Id }, bandsToReturn);
        }

        [HttpGet(Name = "GetBandsByIds")]
        public async Task<ActionResult<IEnumerable<BandDto>>> GetBandsByIds([FromBody] IEnumerable<Guid> bandIds)
        {
            if (bandIds == null)
                throw new ArgumentNullException(nameof(bandIds));

            var bandsFromRepo = await _bandAlbumRepository.GetBands(bandIds);

            return Ok(_mapper.Map<IEnumerable<BandDto>>(bandsFromRepo));
        }

        [HttpGet("({bandIds})")]
        public async Task<ActionResult<IEnumerable<BandDto>>> GetBandCollection([FromRoute] [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> bandIds)
        {
            /*
                Custom Binding the Model in this case
             */
            if (bandIds == null)
                return BadRequest();

            var bandsFromRepo = await _bandAlbumRepository.GetBands(bandIds);

            if (bandIds.Count() != bandsFromRepo.Count())
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<BandDto>>(bandsFromRepo));
        }
    }
}
