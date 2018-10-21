namespace IRunesWebApp.Services
{
    using Data;
    using Models;
    using Services.Contracts;

    using System;
    using System.Linq;

    public class TrackService : ITrackService
    {
        private readonly IRunesDbContext context;

        public TrackService(IRunesDbContext context)
        {
            this.context = context;
        }

        public void CreateTrack(string name, string link, decimal price, string albumId)
        {
            Track track = new Track()
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Link = link,
                Price = price,
                AlbumId = albumId
            };

            this.context.Tracks.Add(track);
            this.context.SaveChanges();
        }

        public Track GetTrackByAlbumTrackIds(string albumId, string trackId)
            => this.context.Tracks.FirstOrDefault(t => t.AlbumId == albumId && t.Id == trackId);
    }
}
