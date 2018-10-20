namespace IRunesWebApp.Controllers
{
    using Services.Contracts;
    using SIS.Framework.ActionResults.Contracts;
    using SIS.Framework.Attributes.Methods;
    using SIS.Framework.Controllers;
    using SIS.HTTP.Common;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;
    using System.IO;
    using System.Linq;
    using ViewModels;

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

        //public IHttpResponse DoRegister(IHttpRequest request)
        //{
        //    string username = request.FormData["username"].ToString().Trim();
        //    string password = request.FormData["password"].ToString();
        //    string confirmPassword = request.FormData["confirmPassword"].ToString();
        //    string email = request.FormData["email"].ToString().Trim();

        //    if (password != confirmPassword)
        //    {
        //        throw new BadRequestException(PasswordsErrorMessage);
        //    }

        //    if (this.dbContext.Users.Any(u => u.Username == username))
        //    {
        //        var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
        //        errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, UsernameTakenErrorMessage);
        //        return new BadRequestResult(errorViewContent);
        //    }
        //    if (this.dbContext.Users.Any(u => u.Email == email))
        //    {
        //        var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
        //        errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, EmailTakenErrorMessage);
        //        return new BadRequestResult(errorViewContent);
        //    }

        //    string hashedPassword = this.hashService.Hash(password);

        //    User user = new User
        //    {
        //        Id = Guid.NewGuid().ToString(),
        //        Username = username,
        //        Password = hashedPassword,
        //        Email = email
        //    };

        //    this.dbContext.Users.Add(user);
        //    try
        //    {
        //        this.dbContext.SaveChanges();
        //    }
        //    catch (System.Exception e)
        //    {
        //        var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
        //        errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, e.Message);
        //        return new BadRequestResult(errorViewContent);
        //    }

        //    var response = new RedirectResult(IndexView);

        //    this.SignInUser(request, response, username);
        //    return response;
        //}

        public IActionResult Logout()
        {
            this.Request.Session.ClearParameters();
            return this.Redirect(IndexView);
        }
    }
}
