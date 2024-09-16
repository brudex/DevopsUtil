using DevopsUtil.Models;
using Microsoft.SemanticKernel;
using System.IO;

namespace DevopsUtil.Services;

public class DockerfileService
{
    private static DockerfileService _instance;
    private static readonly object _lock = new object();

    private DockerfileService() { }

    public static DockerfileService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new DockerfileService();
                    }
                }
            }
            return _instance;
        }
    }

    public async Task<PromptResponse> ExecutePrompt(string userInput)
    {
        Console.WriteLine("Executing ExecuteMenuExpertPrompt...");
        var response = new PromptResponse();
        response.status = PromptResponse.StatusCodesPromptError;
        var kernel = KernelConfig.GetKernelInstance();
        var plugin = kernel.GetSkill("Devops");
        
        string result = string.Empty;
        try
        {
            var translateContent = await kernel.ExecuteFunction(plugin, "DockerfileDesign",
                new() { ["input"] = userInput});
            result = translateContent.GetValue<string>();
            Console.WriteLine($"Prompt result >>> {result}");
            response.status = PromptResponse.StatusCodesSuccess;
            response.message = result;
            return response;
        }
        catch (HttpOperationException ex)
        {
            Logger.Error(this, "Error connecting to ai service " + ex.Message,ex);
            Console.WriteLine("Error connecting to ai service " + ex.Message);
            result = "Error connecting to ai service " + ex.Message;
            response.status = PromptResponse.StatusCodesModelUnreachable;
            response.message = result;
        }
        catch (Exception ex)
        {
            Logger.Error(this, "Error connecting to ai service " + ex.Message,ex);
            Console.WriteLine("Error connecting to ai service " + ex.Message);
            result = "Error connecting to ai service " + ex.Message;
            response.status = PromptResponse.StatusCodesException;
            response.message = result;
        }
        return response;
    }

    public async Task<PromptResponse> ExecutePromptAndWriteOutput(string userInput, ProjectStructure projectStructure)
    {
        var response = await ExecutePrompt(userInput);
        if (response.status == PromptResponse.StatusCodesSuccess)
        {
            try
            {
                await File.WriteAllTextAsync(projectStructure.ProjectPath, response.message);
                Console.WriteLine($"Dockerfile written to: {projectStructure.ProjectPath}");
            }
            catch (Exception ex)
            {
                Logger.Error(typeof(DockerfileService), $"Error writing Dockerfile to {projectStructure.ProjectPath}: {ex.Message}", ex);
                response.status = PromptResponse.StatusCodesException;
                response.message = $"Error writing Dockerfile: {ex.Message}";
            }
        }
        return response;
    }
    
    public async Task<PromptResponse> ExecuteGenericPrompt()
    {
        string userInput = "Create a Dockerfile";
        var response = new PromptResponse();
        response.status = PromptResponse.StatusCodesSuccess;
        response.message = "Dockerfile updated";
        return await Task.FromResult(response);
    }
}