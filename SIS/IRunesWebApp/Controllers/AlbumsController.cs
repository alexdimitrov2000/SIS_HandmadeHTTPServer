namespace IRunesWebApp.Controllers
{
    using Models;
    using SIS.HTTP.Common;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class AlbumsController : BaseController
    {
        private const string AllView = "All";
        private const string CreateView = "Create";
        private const string DetailsView = "Details";
        private const string IndexView = "/";
        private const string AlbumHrefTag = "<a href=\"/Albums/Details?id=@albumId\" ><strong>@albumName</strong></a><br /><br />";
        private const string AlbumImageTag = "<img src=\"@albumImageUrl\" />";
        private const string TrackTag = "<li><a href=\"/Tracks/Details?albumId=@albumId&trackId=@trackId\" ><em>@trackName</em></a></li>";
        private const string CoverParam = "Cover";
        private const string NameParam = "Name";
        private const string PriceParam = "Price";
        private const string AlbumIdParam = "AlbumId";
        private const string TracksContentParam = "TracksContent";
        private const string AlbumsContentParam = "AlbumsContent";

        public IHttpResponse All()
        {
            var albums = this.dbContext.Albums.ToList();
            var sb = new StringBuilder();
            foreach (var album in albums)
            {
                var line = AlbumHrefTag.Replace("@albumId", album.Id).Replace("@albumName", album.Name);
                sb.AppendLine(line);
            }

            var parameters = new Dictionary<string, string>
            {
                { AlbumsContentParam, sb.ToString() }
            };

            return this.View(AllView, parameters);
        }

        public IHttpResponse Create()
        {
            return this.View(CreateView);
        }

        public IHttpResponse Details(IHttpRequest request)
        {
            var albumId = request.QueryData["id"].ToString();
            var album = this.dbContext.Albums.FirstOrDefault(a => a.Id == albumId);

            string imageUrl = AlbumImageTag.Replace("@albumImageUrl", album.Cover);

            var parameters = new Dictionary<string, string>()
            {
                { CoverParam, imageUrl },
                { NameParam, album.Name },
                { PriceParam, album.Price.ToString("F2") },
                { AlbumIdParam, albumId }
            };

            var sbTracks = new StringBuilder();
            foreach (var track in album.Tracks)
            {
                var line = TrackTag.Replace("@albumId", albumId).Replace("@trackId", track.Id).Replace("@trackName", track.Name);
                sbTracks.AppendLine(line);
            }

            string tracksContent = sbTracks.ToString();
            parameters.Add(TracksContentParam, tracksContent);

            return this.View(DetailsView, parameters);
        }

        public IHttpResponse DoCreate(IHttpRequest request)
        {
            string name = request.FormData["name"].ToString();
            string cover = request.FormData["cover"].ToString();

            Album album = new Album
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Cover = cover
            };

            this.dbContext.Albums.Add(album);
            try
            {
                this.dbContext.SaveChanges();
            }
            catch (System.Exception e)
            {
                var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
                errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, e.Message);
                return new BadRequestResult(errorViewContent);
            }

            return new RedirectResult(AllView);
        }
    }
}
