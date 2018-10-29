namespace IRunesWebApp.Controllers
{
    using ViewModels;
    using SIS.Framework.Controllers;
    using SIS.Framework.ActionResults.Contracts;

    public class HomeController : Controller
    {
        private const string LoggedIndexView = "LoggedIndex";
        private const string LoggedIndexViewModel = "LoggedIndexViewModel";

        public IActionResult Index()
        {
            if (this.IsAuthenticated())
            {
                var loggedUser = this.Identity;
                this.Model.Data[LoggedIndexViewModel] = new LoggedIndexViewModel() { Username = loggedUser.Username };
                return this.View(LoggedIndexView);
            }

            return View();
        }
    }
}
