using BandAPI.DbContexts;
using BandAPI.Entities;
using BandAPI.Helpers;
using BandAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.Services
{
    public class BandAlbumRepository : IBandAlbumRepository
    {
        // Bring in the DbContext

        // Repository is more like the functions in models, we define how to interact with DB here.

        private readonly BandAlbumContext _context; // _ for the private
        private readonly IPropertyMappingService _propertyMappingService;

        public BandAlbumRepository(BandAlbumContext context, IPropertyMappingService propertyMappingService)
        {

            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMappingService = propertyMappingService; //  ?? throw new ArgumentNullException(nameof(propertyMappingService));

        }

        public void AddAlbum(Guid bandId, Album album)
        {
            if (bandId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(bandId));
            }

            if (album == null)
            {
                throw new ArgumentNullException(nameof(album));
            }

            album.BandId = bandId;
            _context.Albums.AddRange(album);
        }

        public void AddBands(IEnumerable<Band> bands)
        {
            if (bands == null)
                throw new ArgumentNullException(nameof(bands));

            _context.AddRange(bands);
        }

        public void AddBand(Band band)
        {
            if (band == null)
                throw new ArgumentNullException(nameof(band));
            _context.Bands.AddRange(band);
        }

        public async Task<bool> AlbumExists(Guid albumId)
        {
            if (albumId == Guid.Empty)
                throw new ArgumentNullException(nameof(albumId));

            return await _context.Albums.AnyAsync(a => a.Id == albumId);
        }

        public async Task<bool> BandExists(Guid bandId)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));

            return await _context.Bands.AnyAsync(b => b.Id == bandId);
        }

        public void DeleteAlbum(Album album)
        {
            if (album == null)
                throw new ArgumentNullException(nameof(album));

            _context.Albums.RemoveRange(album);
        }

        public void DeleteBand(Band band)
        {
            if (band == null)
                throw new ArgumentNullException(nameof(band));
            _context.Bands.RemoveRange(band);

        }

        public async Task<Album> GetAlbum(Guid bandId, Guid albumId)
        {

            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));

            if (albumId == null)
                throw new ArgumentNullException(nameof(albumId));

            return await _context.Albums.FirstOrDefaultAsync(a => a.BandId == bandId && a.Id == albumId); // what's the default value in this
        }

        // get albums for a specific band
        public async Task<IEnumerable<Album>> GetAlbums(Guid bandId)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));
            return await _context.Albums.Where(a => a.BandId == bandId).OrderBy(a => a.Title).ToListAsync();
        }

        public async Task<Band> GetBand(Guid bandId)
        {
            if (bandId == Guid.Empty)
                throw new ArgumentNullException(nameof(bandId));

            //return _context.Bands.Include(b => b.Albums).FirstOrDefault(b => b.Id == bandId);
            return await _context.Bands.FirstOrDefaultAsync(b => b.Id == bandId);

        }



        // Get all Bands
        public async Task<IEnumerable<Band>> GetBands()
        {
            return await _context.Bands.ToListAsync();
        }

        public async Task<IEnumerable<Band>> GetBands(IEnumerable<Guid> bandIds)
        {
            if (bandIds == null)
                throw new ArgumentNullException(nameof(bandIds));

            return await _context.Bands.Where(b => bandIds.Contains(b.Id)).OrderBy(b => b.Name).ToListAsync();
        }


        public PageList<Band> GetBands(BandResourceParameters bandResourceParameters)
        {
            if (bandResourceParameters == null)
                throw new ArgumentNullException(nameof(bandResourceParameters));

            //if (string.IsNullOrWhiteSpace(bandResourceParameters.MainGenre) && string.IsNullOrWhiteSpace(bandResourceParameters.SearchQuery))
            //    return await GetBands();

            var collection = _context.Bands as IQueryable<Band>;

            if (!string.IsNullOrWhiteSpace(bandResourceParameters.MainGenre))
            {
                // Doing mainGenre filtering
                var mainGenre = bandResourceParameters.MainGenre.Trim();
                collection = collection.Where(b => b.MainGenre == mainGenre);
            }

            if (!string.IsNullOrWhiteSpace(bandResourceParameters.SearchQuery))
            {
                var searhcQuery = bandResourceParameters.SearchQuery.Trim().ToLower();
                collection = collection.Where(b => b.Name.ToLower().Contains(searhcQuery));
            }

            // Do Sorting here

            if (!string.IsNullOrWhiteSpace(bandResourceParameters.OrderBy))
            {
                var bandPropertyMappingDictionary = _propertyMappingService.GetPropertyMappig<BandDto, Band>(); // Get the band PropertyMapping
                collection = collection.ApplySort(bandResourceParameters.OrderBy, bandPropertyMappingDictionary);
            }


            return PageList<Band>.Create(collection, bandResourceParameters.PageNumber, bandResourceParameters.PageSize);

            //return await collection
            //    .Skip(bandResourceParameters.PageSize * (bandResourceParameters.PageNumber - 1)) // If it's the first page, then skip 0
            //    .Take(bandResourceParameters.PageSize)
            //    .ToListAsync();
        }

        // We need this method to save to the Database
        public async Task<bool> Save()
        {
            return ((await _context.SaveChangesAsync()) >= 0);  // if the SaveChanges returns negative int, then it fail to save 
        }

        public void UpdateAlbum(Album album)
        {
            // No need for implementing
            // Needed for interface

        }

        public void UpdateBand(Band band)
        {
            // No need for implementing
            // Needed for interface
        }
    }
}
