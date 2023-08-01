using NanyPet.Api.Models.Specifications;
using System.Linq.Expressions;

namespace DataAccessLayer.Repositories.Generic
{
    public interface IRepository<T> where T : class
    {
        Task Create(T entity);
        Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null);
        PagedList<T> GetAllPaginated(Parameters parameters, Expression<Func<T, bool>>? filter = null);
        Task<T?> GetById(Expression<Func<T, bool>>? filter = null, bool tracked = true);
        Task Update(T entity);
        Task Delete(T entity);
    }
}
