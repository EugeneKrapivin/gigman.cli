using GigyaManagement.CLI.Services.GigyaApi.Models;

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

public class AccountsSchemaResource : ProjectResource<AccountsSchema>, IPersistable, ILoadable<AccountsSchemaResource>
{
    const string _folderName = "accounts_schemas";
    const string _dataSchemaFile = "schema.data.json";
    const string _profileSchemaFile = "schema.profile.json";
    const string _subscriptionSchemaFile = "schema.subscriptions.json";
    const string _preferencesSchemaFile = "schema.preferences.json";

    [JsonIgnore]
    public static string ConfigFileName => $"site.{_folderName}.json";

    public static async Task<AccountsSchemaResource> Load(string path)
    {
        var confPath = Path.Combine(path, ConfigFileName);
        
        if (!File.Exists(confPath))
            return null;

        var onDisk = JsonSerializer.Deserialize<OnDiskModel>(await File.ReadAllTextAsync(confPath));

        var r = new AccountsSchema
        {
            DataSchema = await GetSchemaFromFile<Schema>(onDisk.Schemas["data"]),
            ProfileSchema = await GetSchemaFromFile<Schema>(onDisk.Schemas["profile"]),
            PreferencesSchema = await GetSchemaFromFile<PreferencesSchema>(onDisk.Schemas["preferences"]),
            SubscriptionsSchema = await GetSchemaFromFile<SubscriptionsSchema>(onDisk.Schemas["subscriptions"])
        };

        return new AccountsSchemaResource
        {
            InheritFrom = onDisk.InheritFrom,
            Resource = r
        };
    }

    private static async Task<T> GetSchemaFromFile<T>(string filePath)
    {
        if (!File.Exists(filePath))
            return default;

        return JsonSerializer.Deserialize<T>(await File.ReadAllTextAsync(filePath));
    }

    public async Task<string> PersistToDisk(string projectPath)
    {
        var folderPath = Path.Combine(projectPath, _folderName);
        
        Directory.CreateDirectory(folderPath);
        
        // TODO: generate json schema and add it to the JSON file for VSc auto completion
        await File.WriteAllTextAsync(GetSchemaPath(folderPath, _dataSchemaFile), JsonSerializer.Serialize(Resource.DataSchema, GlobalUsings.JsonSerializerOptions));
        await File.WriteAllTextAsync(GetSchemaPath(folderPath, _profileSchemaFile), JsonSerializer.Serialize(Resource.ProfileSchema, GlobalUsings.JsonSerializerOptions));
        await File.WriteAllTextAsync(GetSchemaPath(folderPath, _subscriptionSchemaFile), JsonSerializer.Serialize(Resource.SubscriptionsSchema, GlobalUsings.JsonSerializerOptions));
        await File.WriteAllTextAsync(GetSchemaPath(folderPath, _preferencesSchemaFile), JsonSerializer.Serialize(Resource.PreferencesSchema, GlobalUsings.JsonSerializerOptions));

        var conf = new OnDiskModel
        {
            InheritFrom = this.InheritFrom,
            Schemas = new()
            {
                ["data"] = GetSchemaPath(folderPath, _dataSchemaFile),
                ["profile"] = GetSchemaPath(folderPath, _profileSchemaFile),
                ["subscriptions"] = GetSchemaPath(folderPath, _subscriptionSchemaFile),
                ["preferences"] = GetSchemaPath(folderPath, _preferencesSchemaFile)
            }

        };

        var content = JsonSerializer.Serialize(conf, GlobalUsings.JsonSerializerOptions);
        var path = Path.Combine(projectPath, ConfigFileName);

        await File.WriteAllTextAsync(path, content);

        return folderPath;
    }

    static string GetSchemaPath(string root, string file) => Path.Combine(root, file);

    private class OnDiskModel
    {
        public string InheritFrom { get; set; }
        public Dictionary<string, string> Schemas { get; set; }
    }

}
