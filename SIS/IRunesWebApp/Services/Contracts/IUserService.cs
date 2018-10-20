namespace IRunesWebApp.Services.Contracts
{
    public interface IUserService
    {
        bool ExistsByUsernameAndPassword(string username, string password);

        void AddUserToDatabase(string username, string password, string email);

        bool ExistsByUsername(string username);

        bool ExistsByEmail(string email);
    }
}
