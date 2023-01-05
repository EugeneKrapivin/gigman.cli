using System.Text.Json;
using System.Text.Json.Serialization;

using GigyaManagement.CLI.Services.Template.ProjectModels.Resources;
using GigyaManagement.Core;

namespace GigyaManagement.CLI.Services.Project.ProjectModels;

public record Variable(string Path, string DefaultValue);

public class TemplateProject
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int Version { get; } = 1;

    public static bool IsTemplate => true;

    [JsonIgnore]
    public Dictionary<string, Variable> Variables { get; set; } = new();

    [JsonIgnore]
    public static string ConfigFileName => "site.project.json";

    [JsonIgnore]
    public static string VariablesFileName => "site.variables.json";

    #region resources
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
    #endregion

    public static async Task<TemplateProject> Load(string path)
    {
        var file = path;

        if (!path.EndsWith(ConfigFileName))
        {
            var files = Directory.GetFiles(path);

            var exists = files.Select(x => Path.GetFileName(x)).Any(x => x == ConfigFileName);

            if (!exists)
            {
                throw new Exception($"load path \"{path}\" doesn't contain a valid solution file.");
            }

            file = Path.Combine(path, ConfigFileName);
        }
        var fileContent = await File.ReadAllTextAsync(file);
        var project = JsonSerializer.Deserialize<TemplateProject>(fileContent, GlobalUsings.JsonSerializerOptions);

        if (project is null)
        {
            throw new Exception($"failed to parse the template file \"{fileContent}\"");
        }

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
        Directory.CreateDirectory(projectPath);

        await SaveVariablesTable(projectPath);
        await SaveSiteResources(projectPath);
        await SaveProjectFile(projectPath);

        return projectPath;
    }

    private async Task SaveProjectFile(string projectPath)
    {
        var content = await Serialize();
        var path = Path.Combine(projectPath, ConfigFileName);

        await File.WriteAllTextAsync(path, content);
    }

    private async Task SaveVariablesTable(string projectPath)
    {
        var path = Path.Combine(projectPath, VariablesFileName);

        var content = JsonSerializer.Serialize(Variables, GlobalUsings.JsonSerializerOptions);

        await File.WriteAllTextAsync(path, content);
    }

    private async Task SaveSiteResources(string projectPath)
    {
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
    }

    public Task<string> Serialize() => Task.FromResult(JsonSerializer.Serialize(this, GlobalUsings.JsonSerializerOptions));
}
