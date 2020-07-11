using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace VeraCustomTriage.Logic.Models
{
    public class LineItem
    {
        public string FlawId { get; set; }
        public string CategoryDescription { get; set; }
        public string Severity { get; set; }
        public DateTime DateFound { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string LineNumber { get; set; }
        public string ModuleName { get; set; }
        public string RemeditationStatus { get; set; }
        public string MitigationStatus { get; set; }
        public string SecurityTeamComments { get; set; }
        public string DevelopmentTeamComments { get; set; }
        public string EstimatedRemediationDate { get; set; }

    }
}
