﻿namespace IRunesWebApp
{
    using Services;
    using SIS.Framework;
    using Services.Contracts;
    using SIS.Framework.Services;

    public class Launcher
    {
        public static void Main(string[] args)
        {
            WebHost.Start(new StartUp());
        }

        private static void ConfigureServices(DependencyContainer dependencyContainer)
        {
            dependencyContainer.RegisterDependency<IHashService, HashService>();
            dependencyContainer.RegisterDependency<IUserService, UserService>();
            dependencyContainer.RegisterDependency<IAlbumService, AlbumService>();
            dependencyContainer.RegisterDependency<ITrackService, TrackService>();
        }
    }
}
