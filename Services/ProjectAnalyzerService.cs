using DevopsUtil.Models;
using DevopsUtil.Utilities;

namespace DevopsUtil.Services
{
    public class ProjectAnalyzerService : IProjectAnalyzerService
    {
        public ProjectStructure AnalyzeProject(string path)
        {
            // Logic to analyze the project structure
            return new ProjectStructure();
        }

        public void GenerateGitLabCI(ProjectStructure structure)
        {
            GitLabCIGenerator.Generate(structure);
        }
    }
}
