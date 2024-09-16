using System.ComponentModel;
using System.Windows.Input;
using Spectre.Console.Cli;
using DevopsUtil.Services;
using Spectre.Console;
using ICommand = Spectre.Console.Cli.ICommand;

namespace DevopsUtil.Commands
{
    
    public class AnalyzeCommandSettings : CommandSettings
    {
        [CommandOption("-p|--path")]
        [Description("Absolute project/source code path")]
        public string? ProjectPath { get; set; }

        [CommandOption("-d|--dockerfile")]
        [Description("Generate Dockerfile")]
        public bool GenerateDockerfile { get; set; }

        [CommandOption("-a|--artifacts")]
        [Description("Generate K8 Artifacts")]
        public bool GenerateArtifacts { get; set; }

        [CommandOption("-t|--artifacts-types")]
        [Description("Specify types of artifacts to generate (e.g., deployments, services, configmaps, secrets)")]
        public string[]? ArtifactsTypes { get; set; }

        [CommandOption("-c|--gitlab-ci")]
        [Description("Generate Pipeline file")]
        public bool GeneratePipeline { get; set; }

        [CommandOption("-i|--interactive")]
        [Description("Run in interactive mode")]
        public bool InteractiveMode { get; set; }
    }

    public class DevopsCommandHandler : AsyncCommand<AnalyzeCommandSettings>
    {
        

        public override async Task<int> ExecuteAsync(CommandContext context, AnalyzeCommandSettings settings)
        {
            
            var validationResult = this.Validate(context: context, settings: settings);
            if (!validationResult.Successful)
            {
                AnsiConsole.MarkupLine($"[red]Error: {validationResult.Message} [/]");
                return 1;
            }
            if(settings.InteractiveMode){
                PromptPlannerService.Instance.Execute();
                return 0;
            }
            // If not in interactive mode, ensure path is provided
            if (!settings.InteractiveMode && string.IsNullOrEmpty(settings.ProjectPath))
            {
                AnsiConsole.MarkupLine("[red]Error: Project path is required when not in interactive mode.[/]");
                return 1;
            }
            var projectPath = settings.ProjectPath ; 
            if(!Path.Exists(projectPath)){
                AnsiConsole.MarkupLine("[red]Error: Invalid Path supplied.[/]");
                return 1;
            } 
            if (settings.GenerateDockerfile)
            {
                AnsiConsole.MarkupLine("[green]Generating Dockerfile...[/]");
                var promptResponse  = await DockerfileService.Instance.ExecuteGenericPrompt();
                // TODO: Implement Dockerfile generation
            }
            
            
            if (settings.GeneratePipeline)
            {
                AnsiConsole.MarkupLine("[green]Generating Pipeline file...[/]");
                var gitlabCiService = GitlabCiService.Instance.ExecuteGenericPrompt();
                // Call the appropriate method on gitlabCiService
            }

            if (settings.GenerateArtifacts)
            {
                if (settings.ArtifactsTypes == null || settings.ArtifactsTypes.Length == 0)
                {
                    AnsiConsole.MarkupLine("[red]Error: You must specify artifact types when using -a.[/]");
                    return 1;
                }

                AnsiConsole.MarkupLine("[green]Generating Artifacts of types: {0}...[/]", string.Join(", ", settings.ArtifactsTypes));
                // Execute K8DeploymentManifestService or K8ConfigmapManifestService based on types
                foreach (var type in settings.ArtifactsTypes)
                {
                    switch (type.ToLower())
                    {
                        case "deployments":
                            var deploymentService = K8DeploymentManifestService.Instance.ExecuteGenericPrompt();
                            // Call the appropriate method on deploymentService
                            break;
                        case "configmaps":
                            var configMapService = K8ConfigmapManifestService.Instance.ExecuteGenericPrompt();
                            // Call the appropriate method on configMapService
                            break;
                        case "secrets":
                            var secretsService = K8SecretsManifestService.Instance.ExecuteGenericPrompt();
                            break;
                        case "service":
                            var serviceService = K8ServiceManifestService.Instance.ExecuteGenericPrompt();
                            break;
                        default:
                            AnsiConsole.MarkupLine("[red]Error: Unknown artifact type '{0}'[/]", type);
                            return 1;
                    }
                }
            }
            
            return 0;
        }

        public ValidationResult Validate(CommandContext context, AnalyzeCommandSettings settings)
        {
            if (!settings.GenerateDockerfile && !settings.GenerateArtifacts && !settings.GeneratePipeline && !settings.InteractiveMode)
            {
                AnsiConsole.MarkupLine("[yellow]No generation options selected. Use -d, -a, or -p to generate specific files.[/]");
                return ValidationResult.Error("At least one generation option must be specified.");
            }
            if (!settings.InteractiveMode && string.IsNullOrEmpty(settings.ProjectPath))
            {
                //AnsiConsole.MarkupLine("");
                return ValidationResult.Error("[red]Error: Project path is required when not in interactive mode. Use -p to specify project path.[/]");
            }
            var projectPath = settings.ProjectPath; 
            if (!Path.Exists(projectPath))
            {
                AnsiConsole.MarkupLine("[yellow]The specified project path does not exist.[/]");
                return ValidationResult.Error("Invalid path supplied.");
            }
            return ValidationResult.Success(); // Return success if all validations pass
        }

        public Task<int> Execute(CommandContext context, AnalyzeCommandSettings settings)
        {
             var result = ExecuteAsync(context, settings).Result;
             return Task.FromResult(result);
        }
    }
}
