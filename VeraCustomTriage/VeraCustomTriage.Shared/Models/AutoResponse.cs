using System.Collections.Generic;

namespace VeraCustomTriage.Shared.Models
{
    public class AutoResponse
    {
        public string Title { get; set; }
        public string Response { get; set; }
        public string ActionToTake { get; set; }
        public List<PropertyCondition> PropertyConditions { get; set; }
    }
}
