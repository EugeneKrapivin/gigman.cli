using GigyaManagement.CLI.Services.GigyaApi.Models;

using System;
using System.Text.Json.Serialization;

namespace GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

public class AccountsSchemaResource : ProjectResource<AccountsSchema>, IPersistable, ILoadable<AccountsSchemaResource>
{
    [JsonIgnore]
    public static string ConfigFileName => "site.accounts_schema.json";

    public static Task<AccountsSchemaResource> Load(string path) => ProjectResource.Load<AccountsSchemaResource>(path);

    public Task<string> PersistToDisk(string projectPath) => PersistToDisk(projectPath, ConfigFileName);
}
