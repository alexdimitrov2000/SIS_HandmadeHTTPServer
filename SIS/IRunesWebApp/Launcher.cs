namespace IRunesWebApp
{
    using Controllers;
    using SIS.HTTP.Enums;
    using SIS.WebServer;
    using SIS.WebServer.Routing;

    public class Launcher
    {
        public static void Main(string[] args)
        {
            ServerRoutingTable serverRoutingTable = new ServerRoutingTable();

            // GET REQUESTS
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/"] 
                    = request => new HomeController().Index(request);
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Home/Index"] 
                    = serverRoutingTable.Routes[HttpRequestMethod.Get]["/"];
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Login"]
                    = request => new UsersController().Login();
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Register"]
                    = request => new UsersController().Register();

            // POST REQUESTS
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/Login"]
                    = request => new UsersController().DoLogin(request);
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/Register"]
                    = request => new UsersController().DoRegister(request);
            Server server = new Server(8000, serverRoutingTable);

            server.Run();
        }
    }
}
