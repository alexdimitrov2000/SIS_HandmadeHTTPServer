namespace IRunesWebApp.Controllers
{
    using ViewModels;
    using SIS.HTTP.Common;
    using Services.Contracts;
    using SIS.Framework.Controllers;
    using SIS.Framework.Attributes.Action;
    using SIS.Framework.Attributes.Methods;
    using SIS.Framework.ActionResults.Contracts;

    using System;
    using System.IO;
    using System.Linq;

    public class AlbumsController : Controller
    {
        private const string AllView = "All";
        private const string AlbumsContentParam = "AlbumsContent";

        private readonly IAlbumService albumService;

        public AlbumsController(IAlbumService albumService)
        {
            this.albumService = albumService;
        }

        [Authorize]
        public IActionResult All()
        {
            var albums = this.albumService.GetAllAlbums();

            var albumsViewModel = albums.Select(a => new AlbumAllViewModel
            {
                Name = a.Name,
                AlbumId = a.Id
            });

            this.Model.Data["AlbumsViewModel"] = albumsViewModel;

            return this.View();
        }

        [Authorize]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(AlbumCreateViewModel model)
        {
            string name = model.Name;
            string cover = model.Cover;

            try
            {
                this.albumService.CreateAlbum(name, cover);
            }
            catch (Exception e)
            {
                var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
                errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, e.Message);
                return this.ThrowError(errorViewContent);
            }
            
            return this.Redirect(AllView);
        }

        [Authorize]
        public IActionResult Details(AlbumDetailsViewModel model)
        {
            var albumId = model.Id;

            var album = this.albumService.GetAlbumById(albumId);
            
            var albumViewModel = new AlbumViewModel
            {
                Name = album.Name,
                Cover = album.Cover,
                Price = album.Price.ToString("F2"),
                AlbumId = album.Id
            };

            var tracksViewModel = new TrackCollectionViewModel
            {
                Tracks = album.Tracks.Select(t => new TracksViewModel
                {
                    Name = t.Name,
                    AlbumId = album.Id,
                    TrackId = t.Id
                })
            };

            this.Model.Data["AlbumViewModel"] = albumViewModel;
            this.Model.Data["TrackCollectionViewModel"] = tracksViewModel;

            return this.View();
        }
    }
}
