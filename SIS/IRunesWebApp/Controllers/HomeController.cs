namespace IRunesWebApp.Controllers
{
    using SIS.Framework.Controllers;
    using SIS.Framework.ActionResults.Contracts;

    public class HomeController : Controller
    {
        private const string UsernameKey = "Username";
        private const string LoggedIndexView = "LoggedIndex";

        public IActionResult Index()
        {
            if (this.IsAuthenticated())
            {
                this.Model.Data[UsernameKey] = this.Request.Session.GetParameter("username");
                return this.View(LoggedIndexView);
            }

            return View();
        }
    }
}
