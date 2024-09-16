using Spectre.Console;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using DevopsUtil.Commands;
using DevopsUtil.Services;
using System;

namespace DevopsUtil
{
    public class Program
    {
        public static int Main(string[] args)
        {
            
            // Setup CommandApp with the registrar
            var commandApp = new CommandApp();
            // Register Commands
            commandApp.Configure(config =>
            {
                config.SetApplicationName(Constants.AppName);
                config.AddCommand<DevopsCommandHandler>("devops");
                config.PropagateExceptions(); // Shows exceptions if they occur
            });

            try
            {
                return commandApp.Run(args);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
                return -99;
            }
        }

        // // Register services in the DI container
        // private static void ConfigureServices(IServiceCollection services)
        // {
        //     services.AddSingleton<IProjectAnalyzerService, ProjectAnalyzerService>();
        // }
    }
 
}
