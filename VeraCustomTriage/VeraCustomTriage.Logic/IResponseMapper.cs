﻿using System.Collections.Generic;
using System.Linq;
using VeracodeService.Models;
using VeraCustomTriage.Shared;
using VeraCustomTriage.Shared.Models;

namespace VeraCustomTriage.Logic
{
    public interface IResponseMapper
    {
        KeyValuePair<FlawType, AutoResponse[]> GetResponse(FlawType flaw);
        FlawType UpdateCategoryName(FlawType flawType);
        bool HaveIBeenMet(FlawType flaw, PropertyCondition condition);
    }

    public class ResponseMapper : IResponseMapper
    {
        private IGenericReadOnlyRepository<AutoResponse> _responseRepository;
        private IGenericReadOnlyRepository<CategoryRename> _renameRepository;

        public ResponseMapper(
            IGenericReadOnlyRepository<AutoResponse> responseRepository,
            IGenericReadOnlyRepository<CategoryRename> renameRepository
            )
        {
            _responseRepository = responseRepository;
            _renameRepository = renameRepository;
        }
        public FlawType UpdateCategoryName(FlawType flawType)
        {
            var rename = _renameRepository.GetAll().SingleOrDefault(x => x.CweId.Equals(flawType.cweid));
            if(rename != null)
                flawType.categoryname = rename.Rename;

            return flawType;
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

        public bool HaveIBeenMet(FlawType flaw, PropertyCondition condition)
            => flaw.GetType().GetProperties()
            .Any(prop => prop.Name.ToLower() == condition.Property.ToLower()
            && $"{prop.GetValue(flaw)}".ToLower().Contains(condition.Condition.ToLower()));
    }
}
