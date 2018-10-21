namespace SIS.Framework.Routers
{
    using HTTP.Common;
    using WebServer.Api;
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;

    using System.Linq;

    public class HttpRouteHandlingContext : IHttpRouterContext
    {
        private readonly IHttpHandler controllerRouter;

        private readonly IHttpHandler resourceRouter;

        public HttpRouteHandlingContext(IHttpHandler controllerRouter, IHttpHandler resourceRouter)
        {
            this.controllerRouter = controllerRouter;
            this.resourceRouter = resourceRouter;
        }

        public IHttpResponse Handle(IHttpRequest request)
        {
            if (this.IsResourceRequest(request.Path))
                return this.resourceRouter.Handle(request);

            return this.controllerRouter.Handle(request);
        }

        private bool IsResourceRequest(string path)
        {
            if (path.Contains(GlobalConstants.Dot))
            {
                var lastIndexOfDot = path.LastIndexOf(GlobalConstants.Dot);

                var resourceExtension = path.Substring(lastIndexOfDot);

                return GlobalConstants.ResourceExtensions.Contains(resourceExtension);
            }

            return false;
        }
    }
}
