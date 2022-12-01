namespace GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

public class ExtensionsResource : ProjectResource<Conf>, IPersistable, ILoadable<ExtensionsResource>
{
    private const string resourceFolder = "extensions";
    public static string ConfigFileName => $"{resourceFolder}.config.json";

    public static Task<ExtensionsResource> Load(string path)
        => ProjectResource.Load<ExtensionsResource>(Path.Combine(path, resourceFolder));

    public async Task<string> PersistToDisk(string projectPath)
    {
        var confFolderPath = Path.Combine(projectPath, resourceFolder);
        Directory.CreateDirectory(confFolderPath);
        return await PersistToDisk(confFolderPath, ConfigFileName);
    }
}
