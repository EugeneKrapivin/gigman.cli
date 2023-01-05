using GigyaManagement.CLI.Services.GigyaApi.Models;

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

public class AccountsSchemaResource : ProjectResource<AccountsSchema>, IPersistable, ILoadable<AccountsSchemaResource>
{
    const string _folderName = "accounts_schemas";
    const string _dataSchemaFile = "schema.data.json";
    const string _dataJsonSchema = "https://gist.githubusercontent.com/EugeneKrapivin/a8adfe1bbac7b0bf80497a456ea91822/raw/fe37ae3e097317a27e5aea193d4ba9ef683420fe/schema.data.json";
    const string _profileSchemaFile = "schema.profile.json";
    const string _subscriptionSchemaFile = "schema.subscriptions.json";
    const string _subsctiptionsJsonSchema = "https://gist.githubusercontent.com/EugeneKrapivin/9196cb41eaf4568af5aa9ffbecd133bf/raw/86af60665816e62cbd3dfec1ef68cd489a4ecd1d/schema.subscriptions.json";
    const string _preferencesSchemaFile = "schema.preferences.json";
    const string _preferencesJsonSchema = "https://gist.githubusercontent.com/EugeneKrapivin/c71d445896a422e70b42035d087d9419/raw/327741c4cf46e3159eeacd2ecb7c178b98baebf3/schema.preferences,json";

    [JsonIgnore]
    public static string ConfigFileName => $"site.{_folderName}.json";

    public static async Task<AccountsSchemaResource> Load(string path)
    {
        var confPath = Path.Combine(path, _folderName, ConfigFileName);

        if (!File.Exists(confPath))
        {
            return Default();
        }

        var onDisk = JsonSerializer.Deserialize<OnDiskModel>(await File.ReadAllTextAsync(confPath), GlobalUsings.JsonSerializerOptions);

        if (onDisk is null) 
        { 
            return Default(); 
        }

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

    private static AccountsSchemaResource Default()
    {
        return new AccountsSchemaResource
        {
            Resource = new()
        };
    }

    private static async Task<T> GetSchemaFromFile<T>(string filePath)
        where T : new()
    {
        if (!File.Exists(filePath))
            return new T();
        
        var ser = JsonSerializer.Deserialize<T>(await File.ReadAllTextAsync(filePath));

        return ser ?? new T();
    }

    public async Task<string> PersistToDisk(string projectPath)
    {
        var folderPath = Path.Combine(projectPath, _folderName);
        
        Directory.CreateDirectory(folderPath);
        
        // TODO: generate json schema and add it to the JSON file for VSc auto completion
        await File.WriteAllTextAsync(GetSchemaPath(folderPath, _dataSchemaFile), SerializeWithSchema(Resource.DataSchema, _dataJsonSchema));
        await File.WriteAllTextAsync(GetSchemaPath(folderPath, _profileSchemaFile), SerializeWithSchema(Resource.ProfileSchema, _dataJsonSchema));
        await File.WriteAllTextAsync(GetSchemaPath(folderPath, _subscriptionSchemaFile), SerializeWithSchema(Resource.SubscriptionsSchema, _subsctiptionsJsonSchema));
        await File.WriteAllTextAsync(GetSchemaPath(folderPath, _preferencesSchemaFile), SerializeWithSchema(Resource.PreferencesSchema, _preferencesJsonSchema));

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
        var path = Path.Combine(folderPath, ConfigFileName);

        await File.WriteAllTextAsync(path, content);

        return folderPath;

        static string SerializeWithSchema<T>(T source, string schemaUrl)
        {
            var obj = JsonObject.Parse(JsonSerializer.Serialize(source))!;
            obj["$schema"] = schemaUrl;

            return obj.ToJsonString(GlobalUsings.JsonSerializerOptions) ?? string.Empty;
        }
    }

    static string GetSchemaPath(string root, string file) => Path.Combine(root, file);

    private class OnDiskModel
    {
        public string? InheritFrom { get; set; }
        public Dictionary<string, string> Schemas { get; set; } = new();
    }

}
