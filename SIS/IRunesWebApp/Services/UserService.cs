namespace IRunesWebApp.Services
{
    using Contracts;
    using Data;
    using IRunesWebApp.Models;
    using System;
    using System.Linq;

    public class UserService : IUserService
    {
        private readonly IRunesDbContext context;
        private readonly IHashService hashService;

        public UserService(IRunesDbContext context, IHashService hashService)
        {
            this.context = context;
            this.hashService = hashService;
        }

        public bool ExistsByUsernameAndPassword(string usernameOrEmail, string password)
        {
            string hashedPassword = this.hashService.Hash(password);

            bool userExists = this.context.Users
                .Any(u => (u.Username == usernameOrEmail || u.Email == usernameOrEmail) && u.Password == hashedPassword);

            return userExists;
        }

        public void AddUserToDatabase(string username, string password, string email)
        {
            string hashedPassword = this.hashService.Hash(password);

            User user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
                Password = hashedPassword,
                Email = email
            };

            this.context.Users.Add(user);
            this.context.SaveChanges();
        }

        public bool ExistsByUsername(string username)
        {
            return this.context.Users.Any(u => u.Username == username);
        }

        public bool ExistsByEmail(string email)
        {
            return this.context.Users.Any(u => u.Email == email);
        }
    }
}
