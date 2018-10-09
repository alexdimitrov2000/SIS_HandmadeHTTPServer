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

    public class TracksController : BaseController
    {
        private const string CreateView = "Create";
        private const string DetailsView = "Details";
        private const string NonexistingAlbumErrorMessage = "Album with the given id was not found!";
        private const string TrackNotFoundErrorMessage = "Track not found!";
        private const string VideoParam = "Video";
        private const string NameParam = "Name";
        private const string PriceParam = "Price";
        private const string AlbumIdParam = "AlbumId";

        public IHttpResponse Create(IHttpRequest request)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "AlbumId", request.QueryData["albumId"].ToString() }
            };

            return this.View(CreateView, parameters);
        }

        public IHttpResponse Details(IHttpRequest request)
        {
            var albumId = request.QueryData["albumId"].ToString();
            var trackId = request.QueryData["trackId"].ToString();
            var track = this.dbContext.Tracks.FirstOrDefault(t => t.AlbumId == albumId && t.Id == trackId);

            if (track == null)
            {
                var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
                errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, TrackNotFoundErrorMessage);
                return new BadRequestResult(errorViewContent);
            }

            var parameters = new Dictionary<string, string>()
            {
                { VideoParam, track.Link },
                { NameParam, track.Name },
                { PriceParam, track.Price.ToString("F2") },
                { AlbumIdParam, albumId }
            };

            return this.View(DetailsView, parameters);
        }

        public IHttpResponse DoCreate(IHttpRequest request)
        {
            string name = request.FormData["name"].ToString();
            string link = request.FormData["link"].ToString();
            decimal price = decimal.Parse(request.FormData["price"].ToString());
            string albumId = request.QueryData["albumId"].ToString();

            if (this.dbContext.Albums.FirstOrDefault(a => a.Id == albumId) == null)
            {
                var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
                errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, NonexistingAlbumErrorMessage);
                return new BadRequestResult(errorViewContent);
            }

            Track track = new Track()
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Link = link,
                Price = price,
                AlbumId = albumId
            };

            this.dbContext.Tracks.Add(track);
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

            return new RedirectResult($"/Albums/Details?id={albumId}");
        }
    }
}
