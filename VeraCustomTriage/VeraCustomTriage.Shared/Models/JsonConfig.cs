using System;
using System.Collections.Generic;
using System.Text;

namespace VeraCustomTriage.Shared.Models
{
    public class JsonConfig
    {
        public List<Template> Templates { get; set; }
        public List<AutoResponse> AutoResponses { get; set; }
    }
}
