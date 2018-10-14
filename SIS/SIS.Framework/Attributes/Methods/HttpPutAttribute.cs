namespace SIS.Framework.Attributes.Methods
{
    public class HttpPutAttribute : HttpMethodAttribute
    {
        public override bool IsValid(string requestMethod)
            => requestMethod.ToUpper() == "PUT";
    }
}
