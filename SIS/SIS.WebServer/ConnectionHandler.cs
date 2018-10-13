namespace SIS.WebServer
{
    using HTTP.Cookies;
    using HTTP.Enums;
    using HTTP.Requests;
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;
    using HTTP.Sessions;
    using Results;
    using Routing;
    using HTTP.Common;
    using HTTP.Exceptions;
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using System.Linq;

    public class ConnectionHandler
    {
        private readonly Socket client;

        private readonly ServerRoutingTable serverRoutingTable;

        public ConnectionHandler(Socket client, ServerRoutingTable serverRoutingTable)
        {
            this.client = client;
            this.serverRoutingTable = serverRoutingTable;
        }

        private void SetResponseSession(IHttpResponse httpResponse, string sessionId)
        {
            if (sessionId != null)
            {
                httpResponse.AddCookie(new HttpCookie(HttpSessionStorage.SessionCookieKey, $"{sessionId};HttpOnly=true"));
            }
        }

        private string SetRequestSession(IHttpRequest request)
        {
            string sessionId = null;

            if (request.Cookies.ContainsCookie(HttpSessionStorage.SessionCookieKey))
            {
                var cookie = request.Cookies.GetCookie(HttpSessionStorage.SessionCookieKey);
                sessionId = cookie.Value;

                request.Session = HttpSessionStorage.GetSession(sessionId);
            }
            else
            {
                sessionId = Guid.NewGuid().ToString();

                request.Session = HttpSessionStorage.GetSession(sessionId);
            }

            return sessionId;
        }

        private async Task<IHttpRequest> ReadRequest()
        {
            var result = new StringBuilder();
            var data = new ArraySegment<byte>(new byte[1024]);

            while (true)
            {
                int numberOfBytesRead = await this.client.ReceiveAsync(data.Array, SocketFlags.None);

                if (numberOfBytesRead == 0)
                    break;

                var bytesAsString = Encoding.UTF8.GetString(data.Array, 0, numberOfBytesRead);
                result.Append(bytesAsString);

                if (numberOfBytesRead < 1023)
                    break;
            }

            if (result.Length == 0)
                return null;

            // debug
            //Console.WriteLine("REQUEST-------------");
            //Console.WriteLine(result);

            return new HttpRequest(result.ToString());
        }

        private IHttpResponse HandleRequest(IHttpRequest httpRequest)
        {
            if (IsResourceRequest(httpRequest.Path))
                return ResourceFile(httpRequest.Path);

            if (!this.serverRoutingTable.Routes.ContainsKey(httpRequest.RequestMethod) || !this.serverRoutingTable.Routes[httpRequest.RequestMethod].ContainsKey(httpRequest.Path))
            {
                string notFoundContent = File.ReadAllText(GlobalConstants.NotFoundFilePath);
                return new BadRequestResult(notFoundContent);
            }

            return this.serverRoutingTable.Routes[httpRequest.RequestMethod][httpRequest.Path].Invoke(httpRequest);
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

        private async Task PrepareResponse(IHttpResponse httpResponse)
        {
            byte[] byteSegments = httpResponse.GetBytes();

            // debug
            string responseString = Encoding.UTF8.GetString(byteSegments);
            //Console.WriteLine("RESPONSE--------------------------");
            //Console.WriteLine(responseString);

            await this.client.SendAsync(byteSegments, SocketFlags.None);
        }

        public async Task ProcessRequestAsync()
        {
            try
            {
                var httpRequest = await this.ReadRequest();

                if (httpRequest != null)
                {
                    string sessionId = this.SetRequestSession(httpRequest);

                    var httpResponse = this.HandleRequest(httpRequest);

                    this.SetResponseSession(httpResponse, sessionId);

                    await this.PrepareResponse(httpResponse);
                }
            }
            catch (BadRequestException bre)
            {
                await this.PrepareResponse(new TextResult(bre.Message, HttpResponseStatusCode.BadRequest));
            }
            catch (Exception e)
            {
                await this.PrepareResponse(new TextResult(e.Message, HttpResponseStatusCode.InternalServerError));
            }

            this.client.Shutdown(SocketShutdown.Both);
        }
    }
}
