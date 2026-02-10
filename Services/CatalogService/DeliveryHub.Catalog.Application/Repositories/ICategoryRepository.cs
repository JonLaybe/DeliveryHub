using DeliveryHub.CatalogService.Domain.Entities;
using Shared.Abstractions.DataAccess;

namespace DeliveryHub.Catalog.Domain.Appliaction.Repositories
{
    public interface ICategoryRepository: IRepository<Category, Guid>
    {
    }
}
