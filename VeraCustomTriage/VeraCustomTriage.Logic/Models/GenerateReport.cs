using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VeraCustomTriage.Logic.Models
{
    public class GenerateReport
    {
        public string Type { get; set; }
        public int AppId { get; set; }
        public int ScanId { get; set; }
    }
}
