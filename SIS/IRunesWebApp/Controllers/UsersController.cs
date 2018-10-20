namespace IRunesWebApp.Controllers
{
    using ViewModels;
    using SIS.HTTP.Common;
    using Services.Contracts;
    using SIS.HTTP.Exceptions;
    using SIS.Framework.Controllers;
    using SIS.Framework.Attributes.Methods;
    using SIS.Framework.ActionResults.Contracts;

    using System.IO;

    public class UsersController : Controller
    {
        private const string IndexView = "/";
        private const string LoginView = "Login";
        private const string RegisterView = "Register";
        private const string PasswordsErrorMessage = "Passwords do not match.";
        private const string UsernameTakenErrorMessage = "Username is already taken.";
        private const string EmailTakenErrorMessage = "There is already a registered user with that email.";
        private const string UserNotFoundErrorMessage = "No users found with the given combination of username/email and password";

        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        public IActionResult Login()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            string usernameOrEmail = model.Username;
            string password = model.Password;

            var userExists = this.userService.ExistsByUsernameAndPassword(usernameOrEmail, password);

            if (!userExists)
            {
                var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
                errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, UserNotFoundErrorMessage);
                return this.ThrowError(errorViewContent);
            }

            this.Request.Session.AddParameter("username", usernameOrEmail);
            return this.Redirect(IndexView);
        }

        public IActionResult Register()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            string username = model.Username;
            string password = model.Password;
            string confirmPassword = model.ConfirmPassword;
            string email = model.Email;

            if (password != confirmPassword)
            {
                throw new BadRequestException(PasswordsErrorMessage);
            }

            if (this.userService.ExistsByUsername(username))
            {
                var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
                errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, UsernameTakenErrorMessage);
                return this.ThrowError(errorViewContent);
            }

            if (this.userService.ExistsByEmail(email))
            {
                var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
                errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, EmailTakenErrorMessage);
                return this.ThrowError(errorViewContent);
            }

            try
            {
                this.userService.AddUserToDatabase(username, password, email);
            }
            catch (System.Exception e)
            {
                var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
                errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, e.Message);
                return this.ThrowError(errorViewContent);
            }

            this.Request.Session.AddParameter("username", username);
            return this.Redirect(IndexView);
        }

        public IActionResult Logout()
        {
            this.Request.Session.ClearParameters();
            return this.Redirect(IndexView);
        }
    }
}
