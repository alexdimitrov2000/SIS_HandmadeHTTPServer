namespace SIS.Framework.Controllers
{
    using Views;
    using Models;
    using Utilities;
    using ActionResults;
    using ActionResults.Contracts;
    using HTTP.Requests.Contracts;

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

        protected IViewable View([CallerMemberName] string caller = "")
        {
            var controllerName = ControllerUtilities.GetControllerName(this);

            var fullyQualifiedName = ControllerUtilities.GetViewFullQualifiedName(controllerName, caller);

            var view = new View(fullyQualifiedName, this.Model.Data);

            return new ViewResult(view);
        }

        protected IRedirectable Redirect(string redirectUrl)
            => new RedirectResult(redirectUrl);
    }
}
