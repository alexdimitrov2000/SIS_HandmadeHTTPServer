namespace IRunesWebApp.Services
{
    using Data;
    using Models;
    using Contracts;

    using System;
    using System.Linq;

    public class AlbumService : IAlbumService
    {
        private readonly IRunesDbContext context;

        public AlbumService(IRunesDbContext context)
        {
            this.context = context;
        }

        public Album GetAlbumById(string albumId)
            => this.context.Albums.FirstOrDefault(a => a.Id == albumId);

        public Album[] GetAllAlbums()
            => this.context.Albums.ToArray();

        public void CreateAlbum(string name, string cover)
        {
            Album album = new Album
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Cover = cover
            };

            this.context.Albums.Add(album);
            this.context.SaveChanges();
        }

        public bool ExistsById(string albumId)
            => this.context.Albums.FirstOrDefault(a => a.Id == albumId) != null;
    }
}
