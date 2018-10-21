namespace IRunesWebApp.Controllers
{
    using ViewModels;
    using SIS.HTTP.Common;
    using Services.Contracts;
    using SIS.Framework.Controllers;
    using SIS.Framework.Attributes.Methods;
    using SIS.Framework.ActionResults.Contracts;

    using System;
    using System.IO;

    public class AlbumsController : Controller
    {
        private const string AllView = "All";
        private const string AlbumsContentParam = "AlbumsContent";

        private readonly IAlbumService albumService;

        public AlbumsController(IAlbumService albumService)
        {
            this.albumService = albumService;
        }

        public IActionResult All()
        {
            var albumsViewContent = this.albumService.GetAlbumsViewContent();

            this.Model.Data[AlbumsContentParam] = albumsViewContent;

            return this.View();
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
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

        public IActionResult Details(AlbumDetailsViewModel model)
        {
            var albumId = model.Id;

            var parameters = this.albumService.GetAlbumDetailsParameters(albumId);

            foreach (var parameter in parameters)
            {
                this.Model.Data[parameter.Key] = parameter.Value;
            }

            return this.View();
        }
    }
}
