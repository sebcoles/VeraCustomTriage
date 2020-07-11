using System.Collections.Generic;
using System.Linq;
using VeracodeService.Models;
using VeraCustomTriage.Shared;
using VeraCustomTriage.Shared.Models;

namespace VeraCustomTriage.Logic
{
    public interface IResponseMapper
    {
        KeyValuePair<FlawType, AutoResponse[]> GetResponse(FlawType flaw);
    }

    public class ResponseMapper : IResponseMapper
    {
        private IGenericReadOnlyRepository<AutoResponse> _responseRepository;

        public ResponseMapper(IGenericReadOnlyRepository<AutoResponse> responseRepository)
        {
            _responseRepository = responseRepository;
        }
        public KeyValuePair<FlawType, AutoResponse[]> GetResponse(FlawType flaw)
        {
            var returnList = new List<AutoResponse>();
            var responses = _responseRepository.GetAll();
            foreach (var response in responses)
            {
                if (response.PropertyConditions.All(x => HaveIBeenMet(flaw, x)))
                {
                    returnList.Add(response);
                }
            }
            return new KeyValuePair<FlawType, AutoResponse[]>(flaw, returnList.ToArray());
        }

        private bool HaveIBeenMet(FlawType flaw, PropertyCondition condition)
            => flaw.GetType().GetProperties()
            .Any(prop => prop.Name.ToLower() == condition.Property.ToLower()
            && prop.GetValue(flaw).ToString().Contains(condition.Condition));
    }
}
