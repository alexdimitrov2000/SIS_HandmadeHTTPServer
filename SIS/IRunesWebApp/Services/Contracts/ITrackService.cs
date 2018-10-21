namespace IRunesWebApp.Services.Contracts
{
    using Models;

    public interface ITrackService
    {
        void CreateTrack(string name, string link, decimal price, string albumId);

        Track GetTrackByAlbumTrackIds(string albumId, string trackId);
    }
}
