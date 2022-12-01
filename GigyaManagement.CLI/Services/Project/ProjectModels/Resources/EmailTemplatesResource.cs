namespace GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

public class EmailTemplatesResource : ProjectResource<Conf>, IPersistable, ILoadable<EmailTemplatesResource>
{
    private const string resourceFolder = "email_templates";
    public static string ConfigFileName => $"{resourceFolder}.config.json";

    public static Task<EmailTemplatesResource> Load(string path)
        => ProjectResource.Load<EmailTemplatesResource>(Path.Combine(path, resourceFolder));

    public async Task<string> PersistToDisk(string projectPath)
    {
        var confFolderPath = Path.Combine(projectPath, resourceFolder);
        Directory.CreateDirectory(confFolderPath);
        return await PersistToDisk(confFolderPath, ConfigFileName);
    }
}
