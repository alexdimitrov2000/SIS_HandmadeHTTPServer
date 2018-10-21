namespace IRunesWebApp.Services.Contracts
{
    using System.Collections.Generic;

    public interface IAlbumService
    {
        string GetAlbumsViewContent();

        IDictionary<string, string> GetAlbumDetailsParameters(string albumId);

        void CreateAlbum(string name, string cover);

        bool ExistsById(string albumId);
    }
}
