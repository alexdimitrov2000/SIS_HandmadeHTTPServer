namespace IRunesWebApp.Controllers
{
    using IRunesWebApp.Models;
    using IRunesWebApp.Services;
    using IRunesWebApp.Services.Contracts;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;
    using System;
    using System.Linq;

    public class UsersController : BaseController
    {
        private readonly IHashService hashService;

        public UsersController()
        {
            this.hashService = new HashService();
        }

        public IHttpResponse Login()
        {
            return this.View("Login");
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
                return new BadRequestResult("User not found");
            }

            var cookieContent = base.cookieService.GetUserCookie(user.Username);

            var response = new RedirectResult("/");
            var cookie = new HttpCookie(".auth-cakes", cookieContent, 7);
            response.Cookies.Add(cookie);
            return response;
        }

        public IHttpResponse Register()
        {
            return this.View("Register");
        }

        public IHttpResponse DoRegister(IHttpRequest request)
        {
            string username = request.FormData["username"].ToString().Trim();
            string password = request.FormData["password"].ToString();
            string confirmPassword = request.FormData["confirmPassword"].ToString();
            string email = request.FormData["email"].ToString().Trim();
            
            if (password != confirmPassword)
            {
                throw new BadRequestException("Passwords do not match.");
            }

            if (this.dbContext.Users.Any(u => u.Username == username))
            {
                return new BadRequestResult("Username is already taken.");
            }
            if (this.dbContext.Users.Any(u => u.Email == email))
            {
                return new BadRequestResult("There is already a registered user with that email.");
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
                return new HtmlResult(e.Message, SIS.HTTP.Enums.HttpResponseStatusCode.InternalServerError);
            }

            var cookieContent = this.cookieService.GetUserCookie(user.Username);

            var response = new RedirectResult("/");
            var cookie = new HttpCookie(".auth-cakes", cookieContent, 7);
            response.Cookies.Add(cookie);
            return response;
        }
    }
}
