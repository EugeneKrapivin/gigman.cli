namespace GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

public class FlowsResource : ProjectResource<Conf>, IPersistable, ILoadable<FlowsResource>
{
    private const string resourceFolder = "flows";
    public static string ConfigFileName => $"{resourceFolder}.config.json";

    public static Task<FlowsResource> Load(string path) 
        => ProjectResource.Load<FlowsResource>(Path.Combine(path, resourceFolder));

    public async Task<string> PersistToDisk(string projectPath)
    {
        var confFolderPath = Path.Combine(projectPath, resourceFolder);
        Directory.CreateDirectory(confFolderPath);
        return await PersistToDisk(confFolderPath, ConfigFileName);
    }
}
