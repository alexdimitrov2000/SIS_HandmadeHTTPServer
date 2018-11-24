namespace IRunesWebApp
{
    using Services;
    using SIS.Framework.Api;
    using Services.Contracts;
    using SIS.Framework.Services.Contracts;
    using SIS.Framework;
    using IRunesWebApp.Data;

    public class StartUp : MvcApplication
    {
        public override void Configure()
        {
            MvcEngine.Configure();
        }

        public override void ConfigureServices(IDependencyContainer dependencyContainer)
        {
            //dependencyContainer.RegisterDependency<IHomeService, HomeService>();
            dependencyContainer.RegisterDependency<IHashService, HashService>();
            dependencyContainer.RegisterDependency<IUserService, UserService>();
            dependencyContainer.RegisterDependency<IAlbumService, AlbumService>();
            dependencyContainer.RegisterDependency<ITrackService, TrackService>();
        }
    }
}
