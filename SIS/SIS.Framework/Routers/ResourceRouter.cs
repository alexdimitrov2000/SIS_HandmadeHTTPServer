namespace SIS.Framework.Routers
{
    using HTTP.Enums;
    using HTTP.Common;
    using WebServer.Api;
    using WebServer.Results;
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;

    using System.IO;

    public class ResourceRouter : IHttpHandler
    {
        public IHttpResponse Handle(IHttpRequest request)
        {
            return this.ResourceFile(request.Path);
        }


        private IHttpResponse ResourceFile(string requestPath)
        {
            string resourcePath = requestPath.Substring(1);

            if (!File.Exists(resourcePath))
            {
                string notFoundContent = File.ReadAllText(GlobalConstants.NotFoundFilePath);
                return new BadRequestResult(notFoundContent);
            }

            var resourceContent = File.ReadAllBytes(resourcePath);

            return new InlineResourceResult(resourceContent, HttpResponseStatusCode.Ok);
        }
    }
}
