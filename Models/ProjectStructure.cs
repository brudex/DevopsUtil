namespace DevopsUtil.Models
{
    public class ProjectStructure
    {
        public string RootPath { get; set; }
        public List<string> Folders { get; set; } = new List<string>();
        public List<string> Files { get; set; } = new List<string>();
    }
}