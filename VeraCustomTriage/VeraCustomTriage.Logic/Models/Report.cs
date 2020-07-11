using System;
using System.Collections.Generic;
using System.Text;
using VeracodeService.Models;
using VeraCustomTriage.Shared.Models;

namespace VeraCustomTriage.Logic.Models
{
    public class Report
    {
        public string Intro { get; set; }
        public KeyValuePair<FlawType, AutoResponse[]>[] FlawsAndResponses { get; set; }
    }
}
