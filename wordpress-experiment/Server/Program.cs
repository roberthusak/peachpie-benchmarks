using System;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace peachpie.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://*:80/")
                .Build();

            host.Run();
        }
    }

    class Startup
    {
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            app.UsePhp(new PhpRequestOptions(scriptAssemblyName: "peachpie"));
            app.UseDefaultFiles();

            // Use static files embedded in the compiled assembly
            var assembly = Assembly.Load(new AssemblyName("peachpie"));
            var fileProvider = new ManifestEmbeddedFileProvider(assembly);
            app.UseStaticFiles(new StaticFileOptions() { FileProvider = fileProvider });
        }
    }
}