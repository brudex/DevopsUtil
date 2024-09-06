using DevopsUtil.Models;

namespace DevopsUtil.Services
{
    public interface IProjectAnalyzerService
    {
        ProjectStructure AnalyzeProject(string path);
        void GenerateGitLabCI(ProjectStructure structure);
    }
}
