using System.Linq;
using System.Threading.Tasks;

namespace VeraCustomTriage.Shared
{
    public interface IGenericRepository<TEntity> : IGenericReadOnlyRepository<TEntity>
    {
        Task<TEntity> Create(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(int Id);
    }
}
