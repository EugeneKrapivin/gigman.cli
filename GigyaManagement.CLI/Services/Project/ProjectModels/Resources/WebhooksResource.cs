namespace GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

public class WebhooksResource : ProjectResource<Conf>, IPersistable, ILoadable<WebhooksResource>
{
    private const string resourceFolder = "webhooks";
    public static string ConfigFileName => $"{resourceFolder}.config.json";

    public static Task<WebhooksResource> Load(string path)
        => ProjectResource.Load<WebhooksResource>(Path.Combine(path, resourceFolder));

    public async Task<string> PersistToDisk(string projectPath)
    {
        var confFolderPath = Path.Combine(projectPath, resourceFolder);
        Directory.CreateDirectory(confFolderPath);
        return await PersistToDisk(confFolderPath, ConfigFileName);
    }
}
