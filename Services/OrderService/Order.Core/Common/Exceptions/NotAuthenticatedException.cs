namespace OrderService.Core.Common.Exceptions
{
    public class NotAuthenticatedException : Exception
    {
        public NotAuthenticatedException() : base("Failed: authentication required.") { }
    }
}
