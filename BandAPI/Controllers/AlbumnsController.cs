using AutoMapper;
using BandAPI.Entities;
using BandAPI.Models;
using BandAPI.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.Controllers
{
    [ApiController]
    [Route("api/bands/{bandId}/albums")]
    public class AlbumnsController : ControllerBase
    {
        private readonly IBandAlbumRepository _bandAlbumRepository;
        private readonly IMapper _mapper;

        public AlbumnsController(IBandAlbumRepository bandAlbumRepository, IMapper mapper)
        {
            _bandAlbumRepository = bandAlbumRepository ?? throw new ArgumentNullException(nameof(bandAlbumRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbumsForBand(Guid bandId)
        {

            if (!await _bandAlbumRepository.BandExists(bandId))
                return NotFound();

            var albumsFromRepo = await _bandAlbumRepository.GetAlbums(bandId);

            return Ok(_mapper.Map<IEnumerable<AlbumDto>>(albumsFromRepo)); // using the autoMapper in Profile here
        }

        [HttpGet("{albumId}", Name = "GetAlbumForBand")]
        public async Task<ActionResult<AlbumDto>> GetAlbumForBand(Guid bandId, Guid albumId)
        {

            if (!await _bandAlbumRepository.BandExists(bandId))
                return NotFound();

            //if (!_bandAlbumRepository.AlbumExists(albumId))
            //    return NotFound();

            var albumFromRepo = await _bandAlbumRepository.GetAlbum(bandId, albumId);
            if (albumFromRepo == null)
                return NotFound();

            return Ok(_mapper.Map<AlbumDto>(albumFromRepo));

        }

        [HttpPost]
        public async Task<ActionResult<AlbumDto>> CreateNewAlbum(Guid bandId, [FromBody] AlbumForCreatingDto album)
        {

            // Checking if the band Exist
            if (!await _bandAlbumRepository.BandExists(bandId))
                return NotFound();

            var albumEntity = _mapper.Map<Album>(album);

            _bandAlbumRepository.AddAlbum(bandId, albumEntity);
            await _bandAlbumRepository.Save();

            var albumToReturn = _mapper.Map<AlbumDto>(albumEntity);

            return CreatedAtRoute(
                "GetAlbumForBand",
                new { bandId = bandId, albumId = albumToReturn.Id },
                albumToReturn
                );
        }

        [HttpPut("{albumId}")]
        public async Task<ActionResult<AlbumDto>> UpdateAlbumForBand(Guid bandId, Guid albumId, [FromBody] AlbumForUpdatingDto album)
        {
            if (!await _bandAlbumRepository.BandExists(bandId))
                return NotFound();

            //if (!await _bandAlbumRepository.AlbumExists(albumId))
            //    return NotFound();


            var albumFromRepo = await _bandAlbumRepository.GetAlbum(bandId, albumId);

            if (albumFromRepo == null)
            {
                var albumToAdd = _mapper.Map<Album>(album);
                albumToAdd.Id = albumId;
                _bandAlbumRepository.AddAlbum(bandId, albumToAdd);

                await _bandAlbumRepository.Save();

                var upsertingAlbumToReturn = _mapper.Map<AlbumDto>(albumToAdd);

                return CreatedAtRoute(
                     "GetAlbumForBand",
                     new { bandId = bandId, albumId = upsertingAlbumToReturn.Id },
                     upsertingAlbumToReturn
                     );
            }

            //var albumEntity = _mapper.Map<Album>(album);

            _mapper.Map(album, albumFromRepo); // replace the albumFromRepo by album

            _bandAlbumRepository.UpdateAlbum(albumFromRepo);
            await _bandAlbumRepository.Save();

            var albumToReturn = _mapper.Map<AlbumDto>(albumFromRepo);

            return CreatedAtRoute(
                "GetAlbumForBand",
                new { bandId = bandId, albumId = albumToReturn.Id },
                albumToReturn
                );
        }

        [HttpPatch("{albumId}")]
        public async Task<ActionResult> PartiallyUpdateAlbumForBand(Guid bandId, Guid albumId, [FromBody]JsonPatchDocument<AlbumForUpdatingDto> patchDocument)
        {
            if (!await _bandAlbumRepository.BandExists(bandId))
                return NotFound();

            var albumFromRepo = await _bandAlbumRepository.GetAlbum(bandId, albumId);

            if (albumFromRepo == null)
            {
                var albumDto = new AlbumForUpdatingDto();
                patchDocument.ApplyTo(albumDto, ModelState); // Aplly to Dto so we can use the restriction set in Dto

                var albumToAdd = _mapper.Map<Album>(albumDto);

                albumToAdd.Id = albumId;
                _bandAlbumRepository.AddAlbum(bandId, albumToAdd);

                await _bandAlbumRepository.Save();

                var upsertingAlbumToReturn = _mapper.Map<AlbumDto>(albumToAdd);

                return CreatedAtRoute(
                     "GetAlbumForBand",
                     new { bandId = bandId, albumId = upsertingAlbumToReturn.Id },
                     upsertingAlbumToReturn
                     );
            }
            /*
             The Strategy doesn't override the current from repo directly.
             1. Get the CurrentData
             2. Transform CurrentData to Dto
             3. Overriding the Dto (Since the Dto has updating restriction)
             4. Using the Dto to override the CurrentData in the DB
             5. Save the Changes
             */

            var albumToPatch = _mapper.Map<AlbumForUpdatingDto>(albumFromRepo); // Firstly, transform the one in DB to the Dto
            patchDocument.ApplyTo(albumToPatch, ModelState); // Then we can apply Changes to current Dto

            if (!TryValidateModel(albumToPatch))
                return ValidationProblem(ModelState);


            _mapper.Map(albumToPatch, albumFromRepo); // Using the updatedDto to override the current from repo
            _bandAlbumRepository.UpdateAlbum(albumFromRepo);
            await _bandAlbumRepository.Save();

            //return NoContent();

            var albumToReturn = _mapper.Map<AlbumDto>(albumFromRepo);

            return CreatedAtRoute(
              "GetAlbumForBand",
              new { bandId = bandId, albumId = albumToReturn.Id },
              albumToReturn
              );
        }


        [HttpDelete("{albumId}")]
        public async Task<ActionResult> DeleteAlbum(Guid bandId, Guid albumId)
        {
            if (!await _bandAlbumRepository.BandExists(bandId))
                return NotFound();


            var albumFromRepo = await _bandAlbumRepository.GetAlbum(bandId, albumId);
            if (albumFromRepo == null)
                return NotFound();

            _bandAlbumRepository.DeleteAlbum(albumFromRepo);
            await _bandAlbumRepository.Save();

            return Ok(_mapper.Map<AlbumDto>(albumFromRepo));

        }







    }
}
