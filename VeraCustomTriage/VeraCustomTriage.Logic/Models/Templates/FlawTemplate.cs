using System;
using System.Collections.Generic;
using System.Text;

namespace VeraCustomTriage.Logic.Models.Templates
{
    public class FlawTemplate
    {
        public string app_name { get; set; }
        public string very_high_flaw_count { get; set; }
        public string high_flaw_count { get; set; }
        public string latest_scan_name { get; set; }
        public string latest_scan_date { get; set; }
    }
}
