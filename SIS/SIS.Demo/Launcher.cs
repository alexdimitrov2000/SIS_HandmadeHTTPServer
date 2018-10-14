namespace SIS.Demo
{
    using WebServer;
    using Framework;
    using Framework.Routers;

    class Launcher
    {
        static void Main(string[] args)
        {
            var server = new Server(8000, new ControllerRouter());

            MvcEngine.Run(server);
        }
    }
}
