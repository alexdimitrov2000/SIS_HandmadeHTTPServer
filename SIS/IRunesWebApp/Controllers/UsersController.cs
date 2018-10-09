namespace IRunesWebApp.Controllers
{
    using Models;
    using Services;
    using Services.Contracts;
    using SIS.HTTP.Common;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;
    using System;
    using System.IO;
    using System.Linq;

    public class UsersController : BaseController
    {
        private const string IndexView = "/";
        private const string LoginView = "Login";
        private const string RegisterView = "Register";
        private const string PasswordsErrorMessage = "Passwords do not match.";
        private const string UsernameTakenErrorMessage = "Username is already taken.";
        private const string EmailTakenErrorMessage = "There is already a registered user with that email.";
        private const string UserNotFoundErrorMessage = "No users found with the given combination of username/email and password";
        private readonly IHashService hashService;

        public UsersController()
        {
            this.hashService = new HashService();
        }

        public IHttpResponse Login()
        {
            return this.View(LoginView);
        }

        public IHttpResponse DoLogin(IHttpRequest request)
        {
            string usernameOrEmail = request.FormData["username"].ToString().Trim();
            string password = request.FormData["password"].ToString();

            string hashedPassword = this.hashService.Hash(password);

            User user = this.dbContext.Users
                .FirstOrDefault(u => (u.Username == usernameOrEmail || u.Email == usernameOrEmail) && u.Password == hashedPassword);

            if (user == null)
            {
                var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
                errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, UserNotFoundErrorMessage);
                return new BadRequestResult(errorViewContent);
            }

            var response = new RedirectResult(IndexView);

            this.SignInUser(request, response, usernameOrEmail);
            return response;
        }

        public IHttpResponse Register()
        {
            return this.View(RegisterView);
        }

        public IHttpResponse DoRegister(IHttpRequest request)
        {
            string username = request.FormData["username"].ToString().Trim();
            string password = request.FormData["password"].ToString();
            string confirmPassword = request.FormData["confirmPassword"].ToString();
            string email = request.FormData["email"].ToString().Trim();

            if (password != confirmPassword)
            {
                throw new BadRequestException(PasswordsErrorMessage);
            }

            if (this.dbContext.Users.Any(u => u.Username == username))
            {
                var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
                errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, UsernameTakenErrorMessage);
                return new BadRequestResult(errorViewContent);
            }
            if (this.dbContext.Users.Any(u => u.Email == email))
            {
                var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
                errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, EmailTakenErrorMessage);
                return new BadRequestResult(errorViewContent);
            }

            string hashedPassword = this.hashService.Hash(password);

            User user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
                Password = hashedPassword,
                Email = email
            };

            this.dbContext.Users.Add(user);
            try
            {
                this.dbContext.SaveChanges();
            }
            catch (System.Exception e)
            {
                var errorViewContent = File.ReadAllText(GlobalConstants.ErrorViewPath);
                errorViewContent = errorViewContent.Replace(GlobalConstants.ErrorModel, e.Message);
                return new BadRequestResult(errorViewContent);
            }

            var response = new RedirectResult(IndexView);

            this.SignInUser(request, response, username);
            return response;
        }

        public IHttpResponse Logout(IHttpRequest request)
        {
            var response = new RedirectResult(IndexView);
            request.Session.ClearParameters();
            return response;
        }
    }
}
