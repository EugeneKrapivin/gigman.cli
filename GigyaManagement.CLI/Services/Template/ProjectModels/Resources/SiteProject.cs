using System.Text.Json;
using System.Text.Json.Serialization;

namespace GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

public sealed class SiteProject : IPersistable
{
    public int Version { get; } = 1;

    public required string ApiKey { get; init; }

    public bool IsTemplate { get; set; } = false;

    [JsonIgnore]
    public SiteConfigResource? SiteConfigResource { get; set; }

    [JsonIgnore]
    public AccountsSchemaResource? AccountsSchemaResource { get; set; }

    [JsonIgnore]
    public ScreenSetsResource? ScreenSetsResource { get; set; }

    public string? InheritFrom { get; set; }

    [JsonIgnore]
    public static string ConfigFileName => "site.project.json";

    public static async Task<SiteProject> Load(string path)
    {
        var file = path;

        if (!path.EndsWith(ConfigFileName))
        {
            var files = Directory.GetFiles(path);
            
            var exists = files.Select(x => Path.GetFileName(x)).Any(x => x == ConfigFileName);
            
            if (!exists)
            {
                throw new Exception($"load path \"{path}\" doesn't contain a valid project file.");
            }

            file = Path.Combine(path, ConfigFileName);
        }
        var fileContent = await File.ReadAllTextAsync(file);
        var project = JsonSerializer.Deserialize<SiteProject>(fileContent, _serializerOptions);

        project.SiteConfigResource = await SiteConfigResource.Load(path);
        project.AccountsSchemaResource = await AccountsSchemaResource.Load(path);
        project.ScreenSetsResource = await ScreenSetsResource.Load(path);

        return project;
    }

    public async Task<string> PersistToDisk(string projectPath)
    {
        var content = await Serialize();
        var path = Path.Combine(projectPath, ConfigFileName);

        File.WriteAllText(path, content);

        await SiteConfigResource?.PersistToDisk(projectPath);
        await AccountsSchemaResource?.PersistToDisk(projectPath);
        await ScreenSetsResource?.PersistToDisk(projectPath);

        return path;
    }

    private static JsonSerializerOptions _serializerOptions = new ()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public Task<string> Serialize() => Task.FromResult(JsonSerializer.Serialize(this, _serializerOptions));
}
