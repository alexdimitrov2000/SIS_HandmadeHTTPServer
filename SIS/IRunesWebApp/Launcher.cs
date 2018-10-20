namespace IRunesWebApp
{
    using Services;
    using Services.Contracts;
    using SIS.Framework;
    using SIS.Framework.Routers;
    using SIS.Framework.Services;
    using SIS.WebServer;

    public class Launcher
    {
        public static void Main(string[] args)
        {
            var dependencyContainer = new DependencyContainer();
            dependencyContainer.RegisterDependency<IHashService, HashService>();
            dependencyContainer.RegisterDependency<IUserService, UserService>();

            var controllerRouter = new ControllerRouter(dependencyContainer);
            var resourceRouter = new ResourceRouter();

            var server = new Server(8000, new HttpRouteHandlingContext(controllerRouter, resourceRouter));

            MvcEngine.Run(server);
        }
    }
}
