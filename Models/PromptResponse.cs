namespace DevopsUtil.Models;

public class PromptResponse
{
    public string status { get; set; }
    public string message { get; set; }
    public string actionData { get; set; }

    public const string StatusCodesSuccess = "00";
    public const string StatusCodesModelUnreachable = "03";
    public const string StatusCodesPromptError= "06";
    public const string StatusCodesException= "09";
}