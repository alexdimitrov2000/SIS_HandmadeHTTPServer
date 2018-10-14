namespace IRunesWebApp.Controllers
{
    using Data;
    using Services;
    using Services.Contracts;
    using SIS.Framework.Controllers;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;
    using System.Collections.Generic;
    using System.IO;

    public abstract class BaseController : Controller
    {
        private const string ModelReplace = "@Model.";

        private const string ViewsFolderName = "Views";

        private const string LayoutFile = "_Layout.html";

        protected readonly IRunesDbContext dbContext;

        protected readonly IUserCookieService cookieService;

        public BaseController()
        {
            this.dbContext = new IRunesDbContext();
            this.cookieService = new UserCookieService();
        }

        private string GetControllerName()
            => this.GetType().Name.Replace("Controller", string.Empty);

        protected bool IsAuthenticated(IHttpRequest request)
            => request.Session.ContainsParameter("username");
        // CHECK IF THAT'S THE RIGHT WAY TO SIGN IN AND IF IT'S NOT TELL ME HOW TO DO IT :) 
        protected void SignInUser(IHttpRequest request, IHttpResponse response, string username)
        {
            request.Session.AddParameter("username", username);
            var userCookieValue = this.cookieService.GetUserCookie(username);
            response.Cookies.Add(new HttpCookie("auth", userCookieValue));
        }

        protected string GetUsername(IHttpRequest request)
        {
            var username = request.Session.GetParameter("username");
            return username.ToString();

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

        private string GetViewContent(string viewName,
            IDictionary<string, string> viewBag)
        {
            var layoutContent = File.ReadAllText($"{ViewsFolderName}/{LayoutFile}");
            var content = File.ReadAllText($"{ViewsFolderName}/{this.GetControllerName()}/{viewName}.html");
            foreach (var item in viewBag)
            {
                content = content.Replace(ModelReplace + item.Key, item.Value);
            }

            layoutContent = layoutContent.Replace("@RenderBody", content);
            
            return layoutContent;
        }
    }
}
