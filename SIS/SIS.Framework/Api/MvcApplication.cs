namespace SIS.Framework.Api
{
    using Contracts;
    using Services.Contracts;

    public abstract class MvcApplication : IMvcApplication
    {
        public abstract void Configure();

        public abstract void ConfigureServices(IDependencyContainer container);
    }
}
