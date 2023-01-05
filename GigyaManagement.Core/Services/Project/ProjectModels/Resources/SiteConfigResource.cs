using GigyaManagement.CLI.Services.GigyaApi.Models;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

public class SiteConfigResource : ProjectResource<SiteConfig>, IPersistable, ILoadable<SiteConfigResource>
{
    [JsonIgnore]
    public static string ConfigFileName => "site.config.json";

    public static Task<SiteConfigResource> Load(string path) => ProjectResource.Load<SiteConfigResource>(path);

    public Task<string> PersistToDisk(string projectPath) => PersistToDisk(projectPath, ConfigFileName);
}
