
namespace VeraCustomTriage.Logic.Models.Templates
{
    public class BinaryTemplate
    {
        public string app_name { get; set; }
        public string list_latest_file_uploads { get; set; }
        public string latest_file_uploads_count { get; set; }
        public string latest_scan_name { get; set; }
        public string latest_scan_date { get; set; }
        public string changed_files_from_last_scan { get; set; }
        public string added_files_from_last_scan { get; set; }
        public string removed_files_from_last_scan { get; set; }
        public string previous_file_uploads_from_last_12_scans_missing { get; set; }
    }
}
