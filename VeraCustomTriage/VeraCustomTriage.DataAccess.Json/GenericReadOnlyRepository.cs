using Microsoft.Extensions.Options;
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
    public class GenericReadOnlyRepository<TEntity> : IGenericReadOnlyRepository<TEntity>
    {
        private string[] _jsonPath;
        private string _typeName;
        private JsonConfig _dbContext;
        private readonly HttpClient client = new HttpClient();

        public GenericReadOnlyRepository(IOptions<EndpointConfiguration> config)
        {
            _jsonPath = new[]{
                    config.Value.Global,
                    config.Value.Team,
                    config.Value.Personal
                };
            _typeName = typeof(TEntity).Name;
            _dbContext = new JsonConfig
            {
                Template = new List<Template>(),
                AutoResponse = new List<AutoResponse>(),
                CategoryRename = new List<CategoryRename>()
            };
            foreach (var path in _jsonPath)
            {
                var current = LoadJson(path).Result;
                _dbContext.AutoResponse.AddRange(current.AutoResponse);
                _dbContext.Template.AddRange(current.Template);
                _dbContext.CategoryRename.AddRange(current.CategoryRename);
            }
        }

        public async Task<JsonConfig> LoadJson(string jsonPath)
        {
            var streamTask = client.GetStreamAsync(jsonPath);
            return await JsonSerializer.DeserializeAsync<JsonConfig>(await streamTask);
        }
        public IQueryable<TEntity> GetAll()
        {
            var field = typeof(JsonConfig).GetProperties().Single(x => x.Name.Equals(_typeName));
            var list = (List<TEntity>)field.GetValue(_dbContext);
            if (list == null)
                return new List<TEntity>().AsQueryable();

            return list.AsQueryable();
        }
    }
}
