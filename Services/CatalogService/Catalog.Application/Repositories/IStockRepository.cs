using DeliveryHub.Catalog.Domain.Entities;
using Shared.Abstractions.DataAccess;

namespace Catalog.Application.Repositories
{
    public interface IStockRepository: IRepository<Stock, Guid>
    {
    }
}
