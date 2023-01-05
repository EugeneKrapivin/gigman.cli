namespace GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

public class SmsTemplatesResource : ProjectResource<Conf>, IPersistable, ILoadable<SmsTemplatesResource>
{
    private const string resourceFolder = "sms_templates";
    public static string ConfigFileName => $"{resourceFolder}.config.json";

    public static Task<SmsTemplatesResource> Load(string path)
        => ProjectResource.Load<SmsTemplatesResource>(Path.Combine(path, resourceFolder));

    public async Task<string> PersistToDisk(string projectPath)
    {
        var confFolderPath = Path.Combine(projectPath, resourceFolder);
        Directory.CreateDirectory(confFolderPath);
        return await PersistToDisk(confFolderPath, ConfigFileName);
    }
}
