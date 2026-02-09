namespace DeliveryHub.Catalog.Domain.Entities
{
    public class Stock
    {
        public Guid ProductId { get; set; }
        public int Total { get; set; }
        public int Reserved { get; set; }

        public int Available => Total - Reserved;
    }

}
