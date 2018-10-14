namespace SIS.Framework.ActionResults
{
    using SIS.Framework.ActionResults.Contracts;

    public class RedirectResult : IRedirectable
    {
        public RedirectResult(string redirectUrl)
        {
            this.RedirectUtl = redirectUrl;
        }

        public string RedirectUtl { get; }

        public string Invoke()
            => this.RedirectUtl;
    }
}
