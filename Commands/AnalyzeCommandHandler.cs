using System.Windows.Input;
using Spectre.Console.Cli;
using DevopsUtil.Services;
using Spectre.Console;
using ICommand = Spectre.Console.Cli.ICommand;

namespace DevopsUtil.Commands
{
    public class AnalyzeCommandHandler : ICommand
    {
        private readonly IProjectAnalyzerService _analyzerService;

        public AnalyzeCommandHandler(IProjectAnalyzerService analyzerService)
        {
            _analyzerService = analyzerService;
        }

        public void Handle()
        {
            var path = AnsiConsole.Ask<string>("Enter the [green]project path[/]:");
            var structure = _analyzerService.AnalyzeProject(path);
            _analyzerService.GenerateGitLabCI(structure);
        }


        public ValidationResult Validate(CommandContext context, CommandSettings settings)
        {
            throw new NotImplementedException();
        }

        public Task<int> Execute(CommandContext context, CommandSettings settings)
        {
            var path = AnsiConsole.Ask<string>("Enter the [green]project path[/]:");
            var structure = _analyzerService.AnalyzeProject(path);
            _analyzerService.GenerateGitLabCI(structure);
            return Task.FromResult(0);
        }
    }
}
