using DevopsUtil.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevopsUtil
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Register services here
            services.AddSingleton<IProjectAnalyzerService, ProjectAnalyzerService>();
        }
    }
}