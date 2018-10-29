namespace SIS.Framework
{
    using Routers;
    using Services;
    using Services.Contracts;
    using Framework.Api.Contracts;
    using SIS.WebServer;

    public static class WebHost
    {
        private const int HostingPort = 8000;

        public static void Start(IMvcApplication application)
        {
            IDependencyContainer container = new DependencyContainer();
            application.ConfigureServices(container);

            var controllerRouter = new HttpRouteHandlingContext(new ControllerRouter(container), new ResourceRouter());
            application.Configure();

            Server server = new Server(HostingPort, controllerRouter);
            server.Run();
        }
    }
}
