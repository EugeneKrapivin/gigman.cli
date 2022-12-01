namespace GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

public class HostedPagesResource : ProjectResource<Conf>, IPersistable, ILoadable<HostedPagesResource>
{
    private const string resourceFolder = "hosted_pages";
    public static string ConfigFileName => $"{resourceFolder}.config.json";

    public static Task<HostedPagesResource> Load(string path)
        => ProjectResource.Load<HostedPagesResource>(Path.Combine(path, resourceFolder));

    public async Task<string> PersistToDisk(string projectPath)
    {
        var confFolderPath = Path.Combine(projectPath, resourceFolder);
        Directory.CreateDirectory(confFolderPath);
        return await PersistToDisk(confFolderPath, ConfigFileName);
    }
}
