namespace SIS.Framework.ActionResults
{
    using Contracts;

    public class ErrorResult : IError
    {
        public ErrorResult(string content)
        {
            this.Content = content;
        }

        public string Content { get; }

        public string Invoke()
        {
            return this.Content;
        }
    }
}
