using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VeracodeService;
using VeraCustomTriage.Logic.Models;
using System.IO;
using VeraCustomTriage.Logic.Models.Templates;
using VeracodeService.Models;
using VeraCustomTriage.Shared;
using VeraCustomTriage.Shared.Models;

namespace VeraCustomTriage.Logic
{
    public interface IReportGenerator
    {
        public string ScanName(GenerateReport generate);
        byte[] GenerateZip(GenerateReport generate, string password);
        byte[] GenerateReport(GenerateReport generate);
        byte[] GenerateFlawEmail(GenerateReport generate);
        byte[] GenerateModulesEmail(GenerateReport generate);
        byte[] GenerateBinariesEmail(GenerateReport generate);
        byte[] GenerateSupportingEmail(GenerateReport generate);
    }

    public class ReportGenerator : IReportGenerator
    {
        private IResponseMapper _responseMapper;
        private IVeracodeRepository _veracodeRepository;
        private IOutputWriter _outputWriter;
        private IGenericReadOnlyRepository<Template> _templateRepository;
        private ITemplateWriter _templateWriter;
        private IZippingService _zippingService;
        public ReportGenerator(IResponseMapper responseMapper,
            IVeracodeRepository veracodeRepository,
            IOutputWriter outputWriter,
            IGenericReadOnlyRepository<Template> templateRepository,
            ITemplateWriter templateWriter,
            IZippingService zippingService
            )
        {
            _responseMapper = responseMapper;
            _veracodeRepository = veracodeRepository;
            _outputWriter = outputWriter;
            _templateRepository = templateRepository;
            _templateWriter = templateWriter;
            _zippingService = zippingService;
        }
        public byte[] GenerateZip(GenerateReport generate, string password)
        {
            var flaws = _veracodeRepository.GetFlaws(generate.ScanId.ToString());
            var report = new Report
            {
                FlawsAndResponses = flaws.Select(_responseMapper.GetResponse).ToArray()
            };
            var xlsx = _outputWriter.Write(report);
            var datas = new List<KeyValuePair<string, MemoryStream>>
            {
                new KeyValuePair<string, MemoryStream>("flaws.xlsx", xlsx)
            };
            var zip = _zippingService.Zip(datas, password);
            return zip.ToArray();
        }

        public byte[] GenerateReport(GenerateReport generate)
        {
            var flaws = _veracodeRepository.GetFlaws(generate.ScanId.ToString());
            var report = new Report
            {
                FlawsAndResponses = flaws.Select(_responseMapper.GetResponse).ToArray()
            };
            var xlsx = _outputWriter.Write(report);
            return xlsx.ToArray();
        }

        public byte[] GenerateFlawEmail(GenerateReport generate)
        {
            var flaws = _veracodeRepository.GetFlaws(generate.ScanId.ToString());
            var app = _veracodeRepository.GetAppDetail(generate.AppId);
            var build = _veracodeRepository.GetBuildDetail(generate.AppId, generate.ScanId);
            var template = new FlawTemplate
            {
                app_name = app.application[0].app_name,
                latest_scan_name = build.build.version,
                latest_scan_date = build.build.launch_date.ToLongDateString(),
                very_high_flaw_count = flaws.Count(x => x.severity.Equals("5") && x.remediation_status.ToLower().Equals("new")).ToString(),
                high_flaw_count = flaws.Count(x => x.severity.Equals("4") && x.remediation_status.ToLower().Equals("new")).ToString(),
            };
            var emailTemplate = _templateRepository.GetAll().Single(x => x.Title.Equals("email")).Text;
            var covertedTemplate = _templateWriter.Write(template, emailTemplate);
            return Encoding.ASCII.GetBytes(covertedTemplate);
        }

        public string ScanName(GenerateReport generate)
        {
            var build = _veracodeRepository.GetBuildDetail(generate.AppId, generate.ScanId);
            return build.build.version;
        }

        public byte[] GenerateModulesEmail(GenerateReport generate)
        {
            var app = _veracodeRepository.GetAppDetail(generate.AppId);
            var latestBuild = _veracodeRepository.GetBuildDetail(generate.AppId, generate.ScanId);
            var latestEntryPoints = _veracodeRepository.GetEntryPoints(generate.ScanId);
            var latestModules = _veracodeRepository
                .GetModules(generate.AppId, generate.ScanId)
                .Select(x => new
                {
                    Name = x.name,
                    Checksum = x.checksum,
                    EntryPoint = latestEntryPoints.Any(y => y.name == x.name)
                });

            var previousBuilds = _veracodeRepository.GetAllBuildsForApp(generate.AppId)
                .OrderByDescending(x => x.build_id);
            var previousBuildId = previousBuilds.Skip(1).Take(1).First().build_id;
            var previousEntryPoints = _veracodeRepository.GetEntryPoints($"{previousBuildId}");
            var previousLatestModules = _veracodeRepository
                .GetModules(generate.AppId, $"{previousBuildId}")
                .Select(x => new
                {
                    Name = x.name,
                    Checksum = x.checksum,
                    EntryPoint = latestEntryPoints.Any(y => y.name == x.name)
                });

            var missing = previousLatestModules.Where(x => !latestModules.Any(y => y.Name == x.Name));

            var template = new ModuleTemplate
            {
                app_name = app.application[0].app_name,
                latest_scan_name = latestBuild.build.version,
                latest_scan_date = latestBuild.build.launch_date.ToLongDateString(),
                latest_entry_points_count = $"{latestModules.Where(x => x.EntryPoint).Count()}",
                list_all_latest_modules_count = $"{latestModules.Count()}",
                list_all_latest_modules = string.Join("\n", latestModules.Select(x => $"{x.Name},EntryPoint={x.EntryPoint}").ToArray()),
                missing_from_previous_scan = string.Join("\n", missing.Select(x => $"{x.Name},EntryPoint={x.EntryPoint}").ToArray()),
            };
            var emailTemplate = _templateRepository.GetAll().Single(x => x.Title.Equals("modules")).Text;
            var covertedTemplate = _templateWriter.Write(template, emailTemplate);
            return Encoding.ASCII.GetBytes(covertedTemplate);
        }

        public byte[] GenerateBinariesEmail(GenerateReport generate)
        {
            var app = _veracodeRepository.GetAppDetail(generate.AppId);
            var latestbuild = _veracodeRepository.GetBuildDetail(generate.AppId, generate.ScanId);
            var latestFiles = _veracodeRepository.GetFilesForBuild(generate.AppId, generate.ScanId);
            var previousBuildId = _veracodeRepository.GetAllBuildsForApp(generate.AppId)
                .OrderByDescending(x => x.build_id)
                .Skip(1).Take(1);
            var previousFiles = _veracodeRepository.GetFilesForBuild(generate.AppId, $"{previousBuildId}");
            var previous11BuildList = _veracodeRepository.GetAllBuildsForApp(generate.AppId)
                .OrderByDescending(x => x.build_id)
                .Skip(1).Take(11);
            List<FileListFileType> previousFilesLast12 = new List<FileListFileType>();

            var addedFromLastScan = latestFiles.Where(x => !previousFiles.Any(y => y.file_name == x.file_name));
            var removedFromLastScan = previousFiles.Where(x => !latestFiles.Any(y => y.file_name == x.file_name));
            var modifiedFromLastScan = previousFiles.Where(x => latestFiles.Any(y => y.file_name == x.file_name && x.file_md5 != y.file_md5));

            foreach (var build in previous11BuildList)
            {
                var buildFiles = _veracodeRepository.GetFilesForBuild(generate.AppId, $"{build.build_id}");
                previousFilesLast12.AddRange(buildFiles);
            }

            var missing = previousFilesLast12.Where(x => !latestFiles.Any(y => y.file_name == x.file_name));

            var template = new BinaryTemplate
            {
                app_name = app.application[0].app_name,
                latest_scan_name = latestbuild.build.version,
                latest_scan_date = latestbuild.build.launch_date.ToLongDateString(),
                list_latest_file_uploads = string.Join("\n", latestFiles.Select(x => x.file_name).ToArray()),
                latest_file_uploads_count = $"{latestFiles.Count()}",
                previous_file_uploads_from_last_12_scans_missing = string.Join("\n", missing.Select(x => x.file_name).ToArray()),
                added_files_from_last_scan = string.Join("\n", addedFromLastScan.Select(x => x.file_name).ToArray()),
                removed_files_from_last_scan = string.Join("\n", removedFromLastScan.Select(x => x.file_name).ToArray()),
                changed_files_from_last_scan = string.Join("\n", modifiedFromLastScan.Select(x => x.file_name).ToArray())
            };
            var emailTemplate = _templateRepository.GetAll().Single(x => x.Title.Equals("binaries")).Text;
            var covertedTemplate = _templateWriter.Write(template, emailTemplate);
            return Encoding.ASCII.GetBytes(covertedTemplate);
        }

        public byte[] GenerateSupportingEmail(GenerateReport generate)
        {
            var app = _veracodeRepository.GetAppDetail(generate.AppId);
            var modules = _veracodeRepository.GetModules(generate.AppId, generate.ScanId);
            var build = _veracodeRepository.GetBuildDetail(generate.AppId, generate.ScanId);
            var errors = modules.SelectMany(x => x.file_issue.Select(f => $"{f.filename}:{f.details}").ToArray());
            var errorStr = string.Join("\n", errors);

            var template = new SupportingTemplate
            {
                app_name = app.application[0].app_name,
                latest_scan_name = build.build.version,
                latest_scan_date = build.build.launch_date.ToLongDateString(),
                list_missing_supporting_files = errorStr,
                count_missing_supporting_files = $"{errors.Count()}"
            };
            var emailTemplate = _templateRepository.GetAll().Single(x => x.Title.Equals("supporting_files")).Text;
            var covertedTemplate = _templateWriter.Write(template, emailTemplate);
            return Encoding.ASCII.GetBytes(covertedTemplate);
        }
    }
}
