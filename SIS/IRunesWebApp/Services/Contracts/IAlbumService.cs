namespace IRunesWebApp.Services.Contracts
{
    using Models;

    public interface IAlbumService
    {
        Album GetAlbumById(string albumId);

        Album[] GetAllAlbums();

        void CreateAlbum(string name, string cover);

        bool ExistsById(string albumId);
    }
}
