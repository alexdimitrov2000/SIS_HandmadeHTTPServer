namespace IRunesWebApp
{
    using Controllers;
    using IRunesWebApp.Data;
    using Microsoft.EntityFrameworkCore;
    using SIS.Framework;
    using SIS.Framework.Routers;
    using SIS.HTTP.Enums;
    using SIS.WebServer;
    using SIS.WebServer.Routing;

    public class Launcher
    {
        public static void Main(string[] args)
        {
            //ServerRoutingTable serverRoutingTable = new ServerRoutingTable();

            //ConfigureRoutes(serverRoutingTable);

            //Server server = new Server(8000, serverRoutingTable);

            //server.Run();

            var server = new Server(8000, new ControllerRouter());

            MvcEngine.Run(server);
        }

        //private static void ConfigureRoutes(ServerRoutingTable serverRoutingTable)
        //{
        //    // GET REQUESTS
        //    serverRoutingTable.Routes[HttpRequestMethod.Get]["/"]
        //            = request => new HomeController().Index(request);
        //    serverRoutingTable.Routes[HttpRequestMethod.Get]["/Home/Index"]
        //            = serverRoutingTable.Routes[HttpRequestMethod.Get]["/"];
        //    serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Login"]
        //            = request => new UsersController().Login();
        //    serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Register"]
        //            = request => new UsersController().Register();
        //    serverRoutingTable.Routes[HttpRequestMethod.Get]["/Users/Logout"]
        //            = request => new UsersController().Logout(request);
        //    serverRoutingTable.Routes[HttpRequestMethod.Get]["/Albums/All"]
        //            = request => new AlbumsController().All();
        //    serverRoutingTable.Routes[HttpRequestMethod.Get]["/Albums/Create"]
        //            = request => new AlbumsController().Create();
        //    serverRoutingTable.Routes[HttpRequestMethod.Get]["/Albums/Details"]
        //            = request => new AlbumsController().Details(request);
        //    serverRoutingTable.Routes[HttpRequestMethod.Get]["/Tracks/Create"]
        //            = request => new TracksController().Create(request);
        //    serverRoutingTable.Routes[HttpRequestMethod.Get]["/Tracks/Details"]
        //            = request => new TracksController().Details(request);

        //    // POST REQUESTS
        //    serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/Login"]
        //            = request => new UsersController().DoLogin(request);
        //    serverRoutingTable.Routes[HttpRequestMethod.Post]["/Users/Register"]
        //            = request => new UsersController().DoRegister(request);
        //    serverRoutingTable.Routes[HttpRequestMethod.Post]["/Albums/Create"]
        //            = request => new AlbumsController().DoCreate(request);
        //    serverRoutingTable.Routes[HttpRequestMethod.Post]["/Tracks/Create"]
        //            = request => new TracksController().DoCreate(request);
        //}
    }
}
