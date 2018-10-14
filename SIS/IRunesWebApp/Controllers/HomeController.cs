namespace IRunesWebApp.Controllers
{
    using SIS.Framework.ActionResults.Contracts;
    using SIS.Framework.Controllers;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using System.Collections.Generic;

    public class HomeController : BaseController
    {
        private const string UsernameKey = "Username";
        private const string LoggedIndexView = "LoggedIndex";
        private const string IndexView = "Index";

        //public IHttpResponse Index(IHttpRequest request)
        //{
        //    if (this.IsAuthenticated(request))
        //    {
        //        var username = this.GetUsername(request);
        //        var parameters = new Dictionary<string, string>
        //        {
        //            { UsernameKey, username }
        //        };
        //        return this.View(LoggedIndexView, parameters);
        //    }

        //    return this.View(IndexView);
        //}

        public IActionResult Index()
        {
            return View();
        }
    }
}
