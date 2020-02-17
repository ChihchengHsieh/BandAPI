using BandAPI.Entities;
using BandAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.Services
{
    public interface IBandAlbumRepository
    {
        public void AddAlbum(Guid bandId, Album album);
        public void AddBands(IEnumerable<Band> bands);
        public void AddBand(Band band);
        public Task<bool> AlbumExists(Guid albumId);
        public Task<bool> BandExists(Guid bandId);
        public void DeleteAlbum(Album album);
        public void DeleteBand(Band band);
        public Task<Album> GetAlbum(Guid bandId, Guid albumId);
        public Task<IEnumerable<Album>> GetAlbums(Guid bandId);
        public Task<Band> GetBand(Guid bandId);
        public Task<IEnumerable<Band>> GetBands();
        public Task<IEnumerable<Band>> GetBands(IEnumerable<Guid> bandIds);
        public PageList<Band> GetBands(BandResourceParameters bandResourceParameters);
        public Task<bool> Save();
        public void UpdateAlbum(Album album);
        public void UpdateBand(Band band);

    }
}
