using System.Text.Json;
using System.Text.Json.Serialization;

using GigyaManagement.CLI.Services.GigyaApi.Configurators;
using GigyaManagement.CLI.Services.Template.ProjectModels.Resources;
using GigyaManagement.Core;

using Mediator;

namespace GigyaManagement.CLI.Services.Project.ProjectModels;

public class SiteProject
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int Version { get; } = 1;

    public required string Apikey { get; init; }

    public bool ChildSite { get; set; } = false;

    [JsonIgnore]
    public List<SiteProject> ChildSites { get; } = new();

    [JsonIgnore]
    public SiteProject? ParentSite { get; set; }

    [JsonIgnore]
    public string? InheritFrom { get; set; }

    [JsonIgnore]
    public static string ConfigFileName => "site.project.json";

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

    public static async Task<SiteProject> Load(string path)
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
        var project = JsonSerializer.Deserialize<SiteProject>(fileContent, GlobalUsings.JsonSerializerOptions);

        if (project is null)
        {
            throw new Exception($"failed to parse the solution file \"{fileContent}\"");
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

    public async Task<string> PersistToDisk(string solutionPath)
    {
        var siteProjectPath = Path.Combine(solutionPath, SiteConfigResource.Resource.BaseDomain);

        Directory.CreateDirectory(siteProjectPath);

        await WriteSiteConfigFile(siteProjectPath);

        await SaveSiteResources(siteProjectPath);

        await HandleChildSites(siteProjectPath);

        return siteProjectPath;
    }

    private async Task HandleChildSites(string siteProjectPath)
    {
        foreach (var childSite in ChildSites)
        {
            await childSite.PersistToDisk(Path.Combine(siteProjectPath, "child_sites"));
        }
    }

    private async Task WriteSiteConfigFile(string siteProjectPath)
    {
        var content = await Serialize();
        var path = Path.Combine(siteProjectPath, ConfigFileName);

        await File.WriteAllTextAsync(path, content);
    }

    private async Task SaveSiteResources(string projectPath)
    {
        Directory.CreateDirectory(projectPath);
        var resources = new List<IPersist?>
        {
            SiteConfigResource,
            AccountsSchemaResource,
            ScreenSetsResource,
            IdentitySecurityResource,
            HostedPagesResource,
            FlowsResource,
            ExtensionsResource,
            EmailTemplatesResource,
            DataFlowsResource,
            SmsTemplatesResource
        };

        foreach(var resource in resources.Where(x => x != null).Cast<IPersist>())
        {
            await resource.PersistToDisk(projectPath);
        }
    }

    public Task<string> Serialize() => Task.FromResult(JsonSerializer.Serialize(this, GlobalUsings.JsonSerializerOptions));
}
