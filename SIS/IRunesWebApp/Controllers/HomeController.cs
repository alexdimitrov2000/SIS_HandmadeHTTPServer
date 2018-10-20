namespace IRunesWebApp.Controllers
{
    using SIS.Framework.ActionResults.Contracts;
    using SIS.Framework.Controllers;

    public class HomeController : Controller
    {
        private const string UsernameKey = "Username";
        private const string LoggedIndexView = "LoggedIndex";
        private const string IndexView = "Index";

        public IActionResult Index()
        {
            if (this.IsAuthenticated())
                return this.View(LoggedIndexView);

            return View();
        }
    }
}
