using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using VeracodeService;
using VeracodeService.Configuration;
using VeracodeService.Http;
using VeracodeService.Security;
using VeraCustomTriage.DataAccess.Mssql;
using VeraCustomTriage.Logic;
using VeraCustomTriage.Services;
using VeraCustomTriage.Services.Configuration;
using VeraCustomTriage.Shared;
using VeraCustomTriage.Shared.Models;

namespace VeraCustomTriage
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddDbContext<ApplicationDbContext>
                (options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
            services.Configure<VeracodeConfiguration>(options => Configuration.GetSection("Veracode").Bind(options));
            services.Configure<ExcelConfiguration>(options => Configuration.GetSection("ExcelConfiguration").Bind(options));
            services.Configure<ZipConfiguration>(options => Configuration.GetSection("ZipConfiguration").Bind(options));
            services.AddScoped<IVeracodeRepository, VeracodeRepository>();
            services.AddTransient<IGenericRepository<AutoResponse>, AutoReponseRepository>();
            services.AddTransient<IGenericRepository<Template>, GenericRepository<Template>>();
            services.AddScoped<ITemplateWriter, TemplateWriter>();
            services.AddScoped<IOutputWriter, ExcelWriter>();
            services.AddScoped<IResponseMapper, ResponseMapper>();
            services.AddScoped<IZippingService, ZippingService>();
            services.AddScoped<IReportGenerator, ReportGenerator>(); 
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
