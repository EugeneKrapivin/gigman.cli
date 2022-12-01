namespace GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

public class DataFlowsResource : ProjectResource<Conf>, IPersistable, ILoadable<DataFlowsResource>
{
    private const string resourceFolder = "data_flows";
    public static string ConfigFileName => $"{resourceFolder}.config.json";

    public static Task<DataFlowsResource> Load(string path)
        => ProjectResource.Load<DataFlowsResource>(Path.Combine(path, resourceFolder));

    public async Task<string> PersistToDisk(string projectPath)
    {
        var confFolderPath = Path.Combine(projectPath, resourceFolder);
        Directory.CreateDirectory(confFolderPath);
        return await PersistToDisk(confFolderPath, ConfigFileName);
    }
}
