namespace SIS.Framework.Controllers
{
    using Views;
    using Models;
    using Utilities;
    using ActionResults;
    using Security.Contracts;
    using ActionResults.Contracts;
    using HTTP.Requests.Contracts;

    using System.IO;
    using System.Runtime.CompilerServices;

    public abstract class Controller
    {
        protected Controller()
        {
            this.Model = new ViewModel();
        }

        public ViewModel Model { get; }

        public Model ModelState { get; } = new Model();

        public IHttpRequest Request { get; set; }

        public IIdentity Identity => (IIdentity)this.Request.Session.GetParameter("auth");

        public ViewEngine ViewEngine { get; } = new ViewEngine();

        protected bool IsAuthenticated()
        {
            return this.Request.Session.ContainsParameter("auth");
        }

        protected IViewable View([CallerMemberName] string actionName = "")
        {
            var controllerName = ControllerUtilities.GetControllerName(this);
            string viewContent = null;

            try
            {
                viewContent = this.ViewEngine.GetViewContent(controllerName, actionName);
            }
            catch (FileNotFoundException e)
            {
                this.Model.Data["Error"] = e.Message;

                viewContent = this.ViewEngine.GetErrorContent();
            }

            string renderedContent = this.ViewEngine.RenderHtml(viewContent, this.Model.Data);

            var view = new View(renderedContent);
            return new ViewResult(view);
        }

        protected IRedirectable Redirect(string redirectUrl)
            => new RedirectResult(redirectUrl);

        protected IError ThrowError(string content)
            => new ErrorResult(content);

        protected void SignIn(IIdentity auth)
        {
            this.Request.Session.AddParameter("auth", auth);
        }

        protected void SignOut()
        {
            this.Request.Session.ClearParameters();
        }
    }
}
