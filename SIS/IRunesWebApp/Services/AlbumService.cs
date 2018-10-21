namespace IRunesWebApp.Services
{
    using Data;
    using Models;
    using Contracts;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class AlbumService : IAlbumService
    {
        private const string AlbumHrefTag = "<a href=\"/Albums/Details?id=@albumId\" ><strong>@albumName</strong></a><br /><br />";
        private const string AlbumImageTag = "<img src=\"@albumImageUrl\" class=\"img-fluid\" alt=\"Responsive image\" />";
        private const string TrackTag = "<li class=\"black-heading\"><a href=\"/Tracks/Details?albumId=@albumId&trackId=@trackId\" ><em>@trackName</em></a></li>";
        private const string CoverParam = "Cover";
        private const string NameParam = "Name";
        private const string PriceParam = "Price";
        private const string AlbumIdParam = "AlbumId";
        private const string TracksContentParam = "TracksContent";
        private const string AlbumsContentParam = "AlbumsContent";

        private readonly IRunesDbContext context;
        private object dbContext;

        public AlbumService(IRunesDbContext context)
        {
            this.context = context;
        }

        public string GetAlbumsViewContent()
        {
            var albums = this.context.Albums.ToList();
            var sb = new StringBuilder();
            foreach (var album in albums)
            {
                var line = AlbumHrefTag.Replace("@albumId", album.Id).Replace("@albumName", album.Name);
                sb.AppendLine(line);
            }

            return sb.ToString();
        }

        public IDictionary<string, string> GetAlbumDetailsParameters(string albumId)
        {
            var album = this.context.Albums.FirstOrDefault(a => a.Id == albumId);

            string imageUrl = AlbumImageTag.Replace("@albumImageUrl", album.Cover);

            var sbTracks = new StringBuilder();
            foreach (var track in album.Tracks)
            {
                var line = TrackTag.Replace("@albumId", albumId).Replace("@trackId", track.Id).Replace("@trackName", track.Name);
                sbTracks.AppendLine(line);
            }

            string tracksContent = sbTracks.ToString();

            var parameters = new Dictionary<string, string>()
            {
                { CoverParam, imageUrl },
                { NameParam, album.Name },
                { PriceParam, album.Price.ToString("F2") },
                { AlbumIdParam, albumId },
                { TracksContentParam, tracksContent }
            };

            return parameters;
        }

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
