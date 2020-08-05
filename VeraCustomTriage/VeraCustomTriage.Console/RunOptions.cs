using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeraCustomTriage.Console
{
    [Verb("triage", HelpText = "Custom triage flaws")]
    public class RunOptions
    {
        [Option('a', "appid", Default = "", Required = true, HelpText = "Veracode Application ID")]
        public string AppId { get; set; }

        [Option('s', "scanid", Default = "", Required = true, HelpText = "Veracode Build/Scan ID")]
        public string ScanId { get; set; }

        [Option('p', "password", Default = false, Required = true, HelpText = "Password for zip file")]
        public string Password { get; set; }

        [Option('x', "prefix", Default = false, Required = true, HelpText = "Prefix the zip file name with a string")]
        public string Prefix { get; set; }

        [Option('f', "filepath", Default = "", Required = false, HelpText = "Location to save encrypted zip")]
        public string FilePath { get; set; }
    }
}
