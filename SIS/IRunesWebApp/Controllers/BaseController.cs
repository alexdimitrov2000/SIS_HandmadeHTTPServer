namespace IRunesWebApp.Controllers
{
    using CakesWebApp.Services;
    using IRunesWebApp.Data;
    using IRunesWebApp.Services.Contracts;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;
    using System.Collections.Generic;
    using System.IO;

    public abstract class BaseController
    {
        protected readonly IRunesDbContext dbContext;

        protected readonly IUserCookieService cookieService;

        public BaseController()
        {
            this.dbContext = new IRunesDbContext();
            this.cookieService = new UserCookieService();
        }

        private const string ViewsFolderName = "Views";

        private string GetControllerName()
            => this.GetType().Name.Replace("Controller", string.Empty);

        protected IHttpResponse View(string viewName)
        {
            string filePath = $"{ViewsFolderName}/{this.GetControllerName()}/{viewName}.html";

            //if (!File.Exists(filePath))
            //{
            //    string notFoundContent = File.ReadAllText(GlobalConstants.NotFoundFilePath);
            //    return new NotFoundResult(notFoundContent);
            //}

            string content = File.ReadAllText(filePath);



            return new HtmlResult(content, HttpResponseStatusCode.Ok);
        }

        protected string GetUsername(IHttpRequest request)
        {
            if (!request.Cookies.ContainsCookie(".auth-cakes"))
            {
                return null;
            }

            var cookie = request.Cookies.GetCookie(".auth-cakes");
            var cookieContent = cookie.Value;
            var userName = this.cookieService.GetUserData(cookieContent);
            return userName;

        }

        protected IHttpResponse View(string viewName, IDictionary<string, string> viewBag = null)
        {
            if (viewBag == null)
            {
                viewBag = new Dictionary<string, string>();
            }

            var allContent = this.GetViewContent(viewName, viewBag);
            return new HtmlResult(allContent, HttpResponseStatusCode.Ok);
        }

        protected IHttpResponse BadRequestError(string errorMessage)
        {
            var viewBag = new Dictionary<string, string>();
            viewBag.Add("Error", errorMessage);
            var allContent = this.GetViewContent("Error", viewBag);

            return new HtmlResult(allContent, HttpResponseStatusCode.BadRequest);
        }

        protected IHttpResponse ServerError(string errorMessage)
        {
            var viewBag = new Dictionary<string, string>();
            viewBag.Add("Error", errorMessage);
            var allContent = this.GetViewContent("Error", viewBag);

            return new HtmlResult(allContent, HttpResponseStatusCode.InternalServerError);
        }

        private string GetViewContent(string viewName,
            IDictionary<string, string> viewBag)
        {
            var layoutContent = File.ReadAllText("Views/_Layout.html");
            var content = File.ReadAllText("Views/" + viewName + ".html");
            foreach (var item in viewBag)
            {
                content = content.Replace("@Model." + item.Key, item.Value);
            }

            var allContent = layoutContent.Replace("@RenderBody()", content);
            return allContent;
        }
    }
}
