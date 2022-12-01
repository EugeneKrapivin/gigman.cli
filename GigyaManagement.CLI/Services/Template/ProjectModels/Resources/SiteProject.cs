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

    [JsonIgnore]
    public DataFlowsResource? DataFlowsResource { get; set; }
    
    [JsonIgnore]
    public SmsTemplatesResource SmsTemplatesResource { get; set; }
    
    [JsonIgnore]
    public EmailTemplatesResource? EmailTemplatesResource { get; set; }
    
    [JsonIgnore]
    public ExtensionsResource? ExtensionsResource { get; set; }
    
    [JsonIgnore]
    public FlowsResource? FlowsResource { get; set; }
    
    [JsonIgnore]
    public HostedPagesResource? HostedPagesResource { get; set; }
    
    [JsonIgnore]
    public IdentitySecurityResource? IdentitySecurityResource { get; set; }

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
        var project = JsonSerializer.Deserialize<SiteProject>(fileContent, GlobalUsings.JsonSerializerOptions);

        project.SiteConfigResource = await SiteConfigResource.Load(path);
        project.AccountsSchemaResource = await AccountsSchemaResource.Load(path);
        project.ScreenSetsResource = await ScreenSetsResource.Load(path);
        project.IdentitySecurityResource = await IdentitySecurityResource.Load(path);
        project.HostedPagesResource = await HostedPagesResource.Load(path);
        project.FlowsResource = await FlowsResource.Load(path);
        project.ExtensionsResource = await ExtensionsResource.Load(path);
        project.EmailTemplatesResource = await EmailTemplatesResource.Load(path);
        project.DataFlowsResource = await DataFlowsResource.Load(path);
        project.SmsTemplatesResource = await SmsTemplatesResource.Load(path);

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
        await IdentitySecurityResource?.PersistToDisk(projectPath);
        await HostedPagesResource?.PersistToDisk(projectPath);
        await FlowsResource?.PersistToDisk(projectPath);
        await ExtensionsResource?.PersistToDisk(projectPath);
        await EmailTemplatesResource?.PersistToDisk(projectPath);
        await DataFlowsResource?.PersistToDisk(projectPath);
        await SmsTemplatesResource?.PersistToDisk(projectPath);

        return path;
    }

    public Task<string> Serialize() => Task.FromResult(JsonSerializer.Serialize(this, GlobalUsings.JsonSerializerOptions));
}
