using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VeraCustomTriage.Shared;
using VeraCustomTriage.Shared.Models;

namespace VeraCustomTriage.DataAccess.Mssql
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly ApplicationDbContext _dbContext;
        public GenericRepository(ApplicationDbContext context)
        {
            _dbContext = context;
        }
        public IQueryable<TEntity> GetAll()
        {
            return _dbContext
                .Set<TEntity>()
                .AsNoTracking();
        }

        public async Task<TEntity> Create(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int Id)
        {
            var entity = _dbContext.Set<TEntity>().SingleOrDefault(x => x.Id == Id);
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
