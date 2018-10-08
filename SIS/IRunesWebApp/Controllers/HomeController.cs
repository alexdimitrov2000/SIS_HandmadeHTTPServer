namespace IRunesWebApp.Controllers
{
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;

    public class HomeController : BaseController
    {
        public IHttpResponse Index(IHttpRequest request)
        {
            string username = this.GetUsername(request);
            if (username != null)
            {
                string result = this.View("LoggedIndex").ToString();

                if (result.Contains(""))
                {

                }

                return new HtmlResult(result, SIS.HTTP.Enums.HttpResponseStatusCode.Ok);
            }

            return this.View("Index");
        }
    }
}
