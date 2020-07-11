using System;
using System.Collections.Generic;
using System.Text;

namespace VeraCustomTriage.Logic.Models.Templates
{
    public class ModuleTemplate
    {
        public string app_name { get; set; }
        public string latest_scan_name { get; set; }
        public string latest_scan_date { get; set; }
        public string latest_entry_points_count { get; set; }
        public string list_all_latest_modules { get; set; }
        public string list_all_latest_modules_count { get; set; }
        public string missing_from_previous_scan { get; set; }
}
}
