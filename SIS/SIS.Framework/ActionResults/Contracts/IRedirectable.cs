namespace SIS.Framework.ActionResults.Contracts
{
    public interface IRedirectable : IActionResult
    {
        string RedirectUtl { get; }
    }
}
