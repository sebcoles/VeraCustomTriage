using System;
using System.Collections.Generic;
using System.Text;

namespace VeraCustomTriage.Logic.Models.Templates
{
    public class SupportingTemplate
    {
        public string app_name { get; set; }
        public string count_missing_supporting_files { get; set; }
        public string list_missing_supporting_files { get; set; }
        public string latest_scan_name { get; set; }
        public string latest_scan_date { get; set; }
    }
}
