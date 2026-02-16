using Shared.Domain.Entities;

namespace Shared.Abstractions.DataAccess
{
    public interface IRepository<T, K>
        where T : BaseEntity<K> where K : struct, IEquatable<K>
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);

        IQueryable<T> GetAll();

        Task<T> GetByIdAsync(K id, CancellationToken cancellationToken);

        Task DeleteAsync(K id, CancellationToken cancellationToken);

        Task<K> CreateAsync(T entity, CancellationToken cancellationToken);

        Task UpdateAsync(T entity, CancellationToken cancellationToken);
    }

    /// <summary>
    ///     A DbContext instance represents a session with the database and can be used to query and save
    ///     instances of your entities. DbContext is a combination of the Unit Of Work and Repository patterns.
    /// </summary>
    public interface IRepository<T>
    {
        /// <summary>
        ///     Asynchronously retrieves entity by id.
        /// </summary>
        /// <param name="id">Identity <see cref="int" /> entity.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous getting operation. The task result contains the found object by id.
        /// </returns>
        /// <exception cref="NotFoundEntityException">
        ///     The entity could not be found when retrieving data from the context.
        /// </exception>
        Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Asynchronously update the entity.
        /// </summary>
        /// <param name="entity">The updatable entity.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous update operation.
        /// </returns>
        /// <exception cref="NotFoundEntityException">
        ///     The entity could not be found when retrieving data from the context.
        /// </exception>
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Delete an asynchronous entity by identifier.
        /// </summary>
        /// <param name="id">Identity <see cref="int" /> entity.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous remove operation. The task result id delete object.
        /// </returns>
        /// <exception cref="NotFoundEntityException">
        ///     The entity could not be found when retrieving data from the context.
        /// </exception>
        Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Asynchronous creation of a new object.
        /// </summary>
        /// <param name="entity">The new object.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the new oblect.
        /// </returns>
        /// <exception cref="NotFoundEntityException">
        ///     The entity could not be found when retrieving data from the context.
        /// </exception>
        Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);


        /// <summary>
        ///     Saves all changes made in this context to the database.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous save operation. The task result contains the
        ///     number of state entries written to the database.
        /// </returns>
        /// <exception cref="DbUpdateException">
        ///     An error is encountered while saving to the database.
        /// </exception>
        /// <exception cref="DbUpdateConcurrencyException">
        ///     A concurrency violation is encountered while saving to the database.
        ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///     This is usually because the data in the database has been modified since it was loaded into memory.
        /// </exception>
        /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
