namespace SIS.WebServer.Api
{
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;

    public interface IHttpRouterContext
    {
        IHttpResponse Handle(IHttpRequest request);
    }
}
