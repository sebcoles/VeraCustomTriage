using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using VeracodeService;
using VeracodeService.Configuration;
using VeraCustomTriage.DataAccess.Json;
using VeraCustomTriage.Logic;
using VeraCustomTriage.Logic.Models;
using VeraCustomTriage.Shared;
using VeraCustomTriage.Shared.Models;

namespace VeraCustomTriage.Console
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
   .SetBasePath(Directory.GetCurrentDirectory())
#if DEBUG
                .AddJsonFile($"appsettings.Development.json", false)
#else
                .AddJsonFile("appsettings.json", false)
#endif
                .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.Configure<EndpointConfiguration>(options => Configuration.GetSection("Endpoint").Bind(options));
            serviceCollection.Configure<VeracodeConfiguration>(options => Configuration.GetSection("Veracode").Bind(options));
            serviceCollection.Configure<ExcelConfiguration>(options => Configuration.GetSection("ExcelConfiguration").Bind(options));
            serviceCollection.AddScoped<IVeracodeRepository, VeracodeRepository>();
            serviceCollection.AddTransient<IGenericReadOnlyRepository<AutoResponse>, GenericReadOnlyRepository<AutoResponse>>();
            serviceCollection.AddTransient<IGenericReadOnlyRepository<Template>, GenericReadOnlyRepository<Template>>();
            serviceCollection.AddScoped<ITemplateWriter, TemplateWriter>();
            serviceCollection.AddScoped<IOutputWriter, ExcelWriter>();
            serviceCollection.AddScoped<IResponseMapper, ResponseMapper>();
            serviceCollection.AddScoped<IZippingService, ZippingService>();
            serviceCollection.AddScoped<IReportGenerator, ReportGenerator>();

            _serviceProvider = serviceCollection.BuildServiceProvider();

            Parser.Default.ParseArguments<RunOptions>(args)
                .MapResult((
                    RunOptions options) => Run(options),
                    errs => HandleParseError(errs));
        }

        static int Run(RunOptions options)
        {
            var reportGenerator = _serviceProvider.GetService<IReportGenerator>();
            var generateReport = new GenerateReport
            {
                AppId = options.AppId,
                ScanId = options.ScanId
            };
            var reportBytes = reportGenerator.GenerateZip(generateReport, options.Password);
            var reportName = reportGenerator.ScanName(generateReport);
            File.WriteAllBytes($"{options.FilePath}{reportName}.zip", reportBytes);
            return 1;
        }

        static int HandleParseError(IEnumerable<Error> errs)
        {
            return 1;
        }
    }
}
