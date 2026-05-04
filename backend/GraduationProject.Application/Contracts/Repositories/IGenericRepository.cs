using System.Linq;
using System.Threading.Tasks;

namespace GraduationProject.Application.Contracts.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> Query(bool track = false);

        Task AddAsync(T entity);

        void Update(T entity);

        void Remove(T entity);
    }
}
