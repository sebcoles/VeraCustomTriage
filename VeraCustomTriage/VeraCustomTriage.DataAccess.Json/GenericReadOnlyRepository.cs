using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using VeraCustomTriage.Shared;
using VeraCustomTriage.Shared.Models;

namespace VeraCustomTriage.DataAccess.Json
{
    public class GenericReadOnlyRepository<TEntity> : IGenericReadOnlyRepository<TEntity> where TEntity : BaseEntity
    {
        private string _jsonPath;
        private string _typeName;
        private JsonConfig _dbContext;
        private readonly HttpClient client = new HttpClient();

        public GenericReadOnlyRepository(string jsonPath)
        {
            _jsonPath = jsonPath;
            _typeName = typeof(TEntity).Name;
            _dbContext = LoadJson(_jsonPath).Result;
        }

        public async Task<JsonConfig> LoadJson(string jsonPath)
        {
            var streamTask = client.GetStreamAsync(jsonPath);
            return await JsonSerializer.DeserializeAsync<JsonConfig>(await streamTask);
        }
        public IQueryable<TEntity> GetAll()
        {
            var field = typeof(JsonConfig).GetFields().Single(x => x.Name.Equals(_typeName));
            return (IQueryable<TEntity>)field.GetValue(_dbContext);
        }
    }
}
