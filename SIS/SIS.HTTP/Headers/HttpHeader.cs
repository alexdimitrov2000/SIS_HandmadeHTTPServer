namespace SIS.HTTP.Headers
{
    public class HttpHeader
    {
        public static string ContentLength = "Content-Length";
        public static string ContentDisposition = "Content-Disposition";

        public HttpHeader(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key { get; }

        public string Value { get; }

        public override string ToString()
        {
            return $"{this.Key}: {this.Value}";
        }
    }
}
