using System.Linq;
using System.Threading.Tasks;

namespace VeraCustomTriage.Shared
{
    public interface IGenericReadOnlyRepository<TEntity>
    {
        IQueryable<TEntity> GetAll();
    }
}
