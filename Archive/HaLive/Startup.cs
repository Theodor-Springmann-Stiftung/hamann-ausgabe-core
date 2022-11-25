using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HaXMLReader;
using HaXMLReader.Interfaces;
using HaDocument.Interfaces;
using Microsoft.AspNetCore.Diagnostics;

namespace HaLive
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc()
                 //.AddRazorPagesOptions(options =>
                 //{
                 //    options.RootDirectory = "/Pages/Main";
                 //    options.Conventions.Folder
                 //})
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddSingleton<ILibrary>(x => HaDocument.Document.Create(new Options()));
            services.AddTransient<IReaderService, ReaderService>();
        }   
       

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseStaticFiles();
            app.UseMvc();
        }
    }

    class Options : IHaDocumentOptions {
        public string HamannXMLFilePath { get; set; } = @"Hamann.xml";
        public string[] AvailableVolumes { get; set; } = {  };
        public bool NormalizeWhitespace { get; set; } = true;
        public (int, int) AvailableYearRange {get; set; } = (1751, 1788);
    }
}
