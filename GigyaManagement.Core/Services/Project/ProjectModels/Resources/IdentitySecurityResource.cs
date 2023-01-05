namespace GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

public class IdentitySecurityResource : ProjectResource<Conf>, IPersistable, ILoadable<IdentitySecurityResource>
{
    private const string resourceFolder = "identity_sec";
    public static string ConfigFileName => $"{resourceFolder}.config.json";

    public static Task<IdentitySecurityResource> Load(string path)
        => ProjectResource.Load<IdentitySecurityResource>(Path.Combine(path, resourceFolder));

    public async Task<string> PersistToDisk(string projectPath)
    {
        var confFolderPath = Path.Combine(projectPath, resourceFolder);
        Directory.CreateDirectory(confFolderPath);
        return await PersistToDisk(confFolderPath, ConfigFileName);
    }
}
