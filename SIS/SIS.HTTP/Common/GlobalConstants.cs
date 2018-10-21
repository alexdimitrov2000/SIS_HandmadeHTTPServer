namespace SIS.HTTP.Common
{
    public static class GlobalConstants
    {
        public const string HttpOneProtocolFragment = "HTTP/1.1";

        public const string HostHeaderKey = "Host";

        public const string NotFoundFilePath = "Views/NotFound.html";

        public const string ErrorViewPath = "Views/Error.html";

        public const string ErrorModel = "@Model.Error";

        public const string ModelParam = "@Model.";

        public static string[] ResourceExtensions = { ".js", ".css", ".ico", ".map" };

        public static string Dot = ".";
    }
}
