using System;
using System.Collections.Generic;
using System.Text;

namespace VeraCustomTriage.Shared.Models
{
    public class FlawFilterConfiguration
    {
        public PropertyConditionCollection[] PropertyConditionCollections { get; set; }
    }

    public class PropertyConditionCollection
    {
        public PropertyCondition[] PropertyConditions { get; set; }

    }
}
