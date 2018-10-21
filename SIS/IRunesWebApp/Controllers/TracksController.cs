namespace IRunesWebApp.Controllers
{
    using Models;
    using ViewModels;
    using SIS.HTTP.Common;
    using Services.Contracts;
    using SIS.Framework.Controllers;
    using SIS.Framework.Attributes.Methods;
    using SIS.Framework.ActionResults.Contracts;

    using System.IO;
    using System.Collections.Generic;

    public class TracksController : Controller
    {
        private const string NonexistingAlbumErrorMessage = "Album with the given id was not found!";
        private const string TrackNotFoundErrorMessage = "Track not found!";
        private const string VideoParam = "Video";
        private const string NameParam = "Name";
        private const string PriceParam = "Price";
        private const string AlbumIdParam = "AlbumId";

        private readonly ITrackService trackService;

        private readonly IAlbumService albumService;

        public TracksController(ITrackService trackService, IAlbumService albumService)
        {
            this.trackService = trackService;
            this.albumService = albumService;
        }

        public IActionResult Create(string albumId)
        {
            this.Model.Data[AlbumIdParam] = albumId;

            return this.View();
        }

        [HttpPost]
        public IActionResult Create(TrackCreateViewModel model)
        {
            string name = model.Name;
            string link = model.Link;
            decimal price = model.Price;
            string albumId = model.AlbumId;
            
            if (!this.albumService.ExistsById(albumId))
            {
                var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
                errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, NonexistingAlbumErrorMessage);
                return this.ThrowError(errorViewContent);
            }

            try
            {
                this.trackService.CreateTrack(name, link, price, albumId);
            }
            catch (System.Exception e)
            {
                var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
                errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, e.Message);
                return this.ThrowError(errorViewContent);
            }

            return this.Redirect($"/Albums/Details?id={albumId}");
        }

        public IActionResult Details(TrackDetailsViewModel model)
        {
            var albumId = model.AlbumId;
            var trackId = model.TrackId;

            Track track = this.trackService.GetTrackByAlbumTrackIds(albumId, trackId);

            if (track == null)
            {
                var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
                errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, TrackNotFoundErrorMessage);
                return this.ThrowError(errorViewContent);
            }

            var parameters = new Dictionary<string, string>()
            {
                { VideoParam, track.Link },
                { NameParam, track.Name },
                { PriceParam, track.Price.ToString("F2") },
                { AlbumIdParam, albumId }
            };

            foreach (var parameter in parameters)
            {
                this.Model.Data[parameter.Key] = parameter.Value;
            }

            return this.View();
        }
    }
}
