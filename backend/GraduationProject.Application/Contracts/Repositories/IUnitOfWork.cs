using System.Threading.Tasks;

namespace GraduationProject.Application.Contracts.Repositories
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> Repository<T>() where T : class;

        Task<int> SaveChangesAsync();
    }
}
