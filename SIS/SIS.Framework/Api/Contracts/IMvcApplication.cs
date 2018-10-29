namespace SIS.Framework.Api.Contracts
{
    using Services.Contracts;

    public interface IMvcApplication
    {
        void Configure();

        void ConfigureServices(IDependencyContainer container);
    }
}
