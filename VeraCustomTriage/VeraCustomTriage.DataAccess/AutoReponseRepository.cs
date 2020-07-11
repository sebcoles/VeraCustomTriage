using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeraCustomTriage.Shared;
using VeraCustomTriage.Shared.Models;

namespace VeraCustomTriage.DataAccess.Mssql
{
    public class AutoReponseRepository : IGenericRepository<AutoResponse>
    {
        protected readonly ApplicationDbContext _dbContext;
        public AutoReponseRepository(ApplicationDbContext context)
        {
            _dbContext = context;
        }
        public IQueryable<AutoResponse> GetAll()
        {
            return _dbContext
                .Set<AutoResponse>()
                .Include(c => c.PropertyConditions)
                .AsNoTracking();
        }

        public async Task<AutoResponse> Create(AutoResponse entity)
        {
            _dbContext.Set<AutoResponse>().Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Update(AutoResponse entity)
        {
            _dbContext.Set<AutoResponse>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int Id)
        {
            var entity = _dbContext
                .Set<AutoResponse>()
                .Include(c => c.PropertyConditions)
                .SingleOrDefault(x => x.Id == Id);
            _dbContext.Set<AutoResponse>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
