using DataAccessLayer.Repositories.Generic;
using NanyPet.Api.Models.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.GenericServices
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly IRepository<T> _iRepository;

        public Service(IRepository<T> iRepository)
        {
            _iRepository = iRepository;

        }
        public async Task Create(T entity)
        {
            await _iRepository.Create(entity);
        }

        public async Task Delete(T entity)
        {
            await _iRepository.Delete(entity);
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null)
        {
            return await _iRepository.GetAll(filter);
        }

        public PagedList<T> GetAllPaginated(Parameters parameters, Expression<Func<T, bool>>? filter = null)
        {
            return _iRepository.GetAllPaginated(parameters, filter);
        }

        public async Task<T?> GetById(Expression<Func<T, bool>>? filter = null, bool tracked = true)
        {
            return await _iRepository.GetById(filter, tracked);

        }

        public async Task Update(T entity)
        {
            await _iRepository.Update(entity);
        }
    }


}
