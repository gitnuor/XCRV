using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace XCRV.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //Log.Logger = new LoggerConfiguration()
            //   .Enrich.FromLogContext()
            //   .WriteTo.Console(new RenderedCompactJsonFormatter())
            //   .WriteTo.Debug(outputTemplate: DateTime.Now.ToString())
            //   .WriteTo.File("D:\\Logs\\LogInfo.txt", rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 200000)
            //   .CreateLogger();

           // Log.Logger = new LoggerConfiguration()
           //.MinimumLevel.Debug()
           //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
           //.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
           //.WriteTo.File("D:\\XCRVLog\\", rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 200000)
           //.CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }
       
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);
                    webBuilder.UseStartup<Startup>();
                    
                });
    }
}
