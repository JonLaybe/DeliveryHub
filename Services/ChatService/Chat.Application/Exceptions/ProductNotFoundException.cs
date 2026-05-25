namespace Chat.Application.Exceptions
{
    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException(Guid productId)
            : base($"Product with id {productId} was not found")
        {
        }
    }
}
