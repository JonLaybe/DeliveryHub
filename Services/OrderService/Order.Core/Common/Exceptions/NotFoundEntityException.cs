namespace OrderService.Core.Common.Exceptions
{
    public class NotFoundEntityException : Exception
    {
        public NotFoundEntityException() { }

        public NotFoundEntityException(string entityName) : base($"Entity \"{entityName}\" not found.") { }
    }
}
