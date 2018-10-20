namespace SIS.Framework.Routers
{
    using SIS.HTTP.Common;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Api;
    using SIS.WebServer.Results;
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
