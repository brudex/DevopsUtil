using Spectre.Console;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using DevopsUtil.Commands;
using DevopsUtil.Services;

namespace DevopsUtil
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();
            var commandApp = new CommandApp();

            // Register Commands
            commandApp.Configure(config =>
            {
                config.PropagateExceptions();
                config.AddCommand<AnalyzeCommandHandler>("analyze");
            }); 

            try
            {
                return  commandApp.Run(args);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
                return -99;
            } 
           
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IProjectAnalyzerService, ProjectAnalyzerService>();
        }
    }
}
