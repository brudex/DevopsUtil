namespace DevopsUtil.Services;

public class FileManagerService
{


    public bool WriteContentToFile(string fullPath, string content)
    {
        try
        {
            File.WriteAllText(fullPath, content);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}