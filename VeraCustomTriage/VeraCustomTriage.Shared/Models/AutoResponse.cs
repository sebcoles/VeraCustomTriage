using System.Collections.Generic;

namespace VeraCustomTriage.Shared.Models
{
    public class AutoResponse : BaseEntity
    {
        public string Title { get; set; }
        public string Response { get; set; }
        public List<PropertyCondition> PropertyConditions { get; set; }
    }
}
