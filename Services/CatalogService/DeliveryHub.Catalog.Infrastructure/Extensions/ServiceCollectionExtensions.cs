using Catalog.Infrastructure.Mongo;
using Shared.Abstractions.DataAccess;
using Shared.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoClient(
            this IServiceCollection services, string connectionString, string dbName)
        {
            services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

            services.AddScoped(sp =>
                sp.GetRequiredService<IMongoClient>()
                  .GetDatabase(dbName));

            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(
            this IServiceCollection services,
            string collectionName)
            where T : BaseEntity<Guid>
        {
            services.AddScoped<IRepository<T, Guid>>(sp =>
            {
                var db = sp.GetRequiredService<IMongoDatabase>();
                return new MongoRepository<T>(db, collectionName);
            });

            return services;
        }
    }

}
