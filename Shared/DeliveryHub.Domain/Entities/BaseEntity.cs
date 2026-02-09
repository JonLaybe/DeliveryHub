namespace DeliveryHub.Domain.Entities
{
    public abstract class BaseEntity<T> where T : struct, IEquatable<T>
    {
        public T Id { get; set; }
    }
}
