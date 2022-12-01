using GigyaManagement.CLI.Services.GigyaApi.Models;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

public class ScreenSetsResource : ProjectResource<ScreenSetsConfig>, IPersistable, ILoadable<ScreenSetsResource>
{
    [JsonIgnore]
    public static string ConfigFileName => "screensets.json";
    const string _folder = "screens_sets";
    public static async Task<ScreenSetsResource> Load(string path)
    {
        path = Path.Combine(path, _folder);

        var resourceConfigContent = await File.ReadAllTextAsync(Path.Combine(path, ConfigFileName));
        var resource = JsonSerializer.Deserialize<ScreenSetsConfigOnDiskModel>(resourceConfigContent, GlobalUsings.JsonSerializerOptions)!;

        List<ScreenSet> screenSets = new();
        foreach (var screenSetId in resource.ScreenSets)
        {
            var folder = Path.Combine(path, screenSetId);

            var conf = await File.ReadAllTextAsync(Path.Combine(folder, $"{screenSetId}.config.json"));
            var screenSet = JsonSerializer.Deserialize<ScreenSet>(conf, GlobalUsings.JsonSerializerOptions);
            screenSet.Html = await File.ReadAllTextAsync(Path.Combine(folder, $"{screenSetId}.html"));
            screenSet.Css = await File.ReadAllTextAsync(Path.Combine(folder, $"{screenSetId}.css"));
            screenSet.Javascript = await File.ReadAllTextAsync(Path.Combine(folder, $"{screenSetId}.js"));
            
            screenSets.Add(screenSet);
        }

        return new ScreenSetsResource
        {
            Resource = new ScreenSetsConfig
            {
                ScreenSets = screenSets.ToArray()
            },
            InheritFrom = resource.InheritFrom
        };
    }

    public async Task<string> PersistToDisk(string projectPath)
    {
        foreach(var screen in Resource.ScreenSets)
        {
            var screenSetId = screen.ScreenSetId;
            var folder = Path.Combine(projectPath, "screens_sets", screenSetId);

            Directory.CreateDirectory(folder);

            await File.WriteAllTextAsync(Path.Combine(folder, $"{screenSetId}.config.json"), SerializeSceenSet(screen));
            await File.WriteAllTextAsync(Path.Combine(folder, $"{screenSetId}.html"), screen.Html);
            await File.WriteAllTextAsync(Path.Combine(folder, $"{screenSetId}.css"), screen.Css);
            await File.WriteAllTextAsync(Path.Combine(folder, $"{screenSetId}.js"), screen.Javascript);
        }

        var path = Path.Combine(projectPath, _folder);
        return await base.PersistToDisk(path, ConfigFileName);

        static string SerializeSceenSet(ScreenSet screenSet)
            => JsonSerializer.Serialize(new ScreenSetOnDiskModel
            {
                ScreenSetId = screenSet.ScreenSetId,
                Translations = screenSet.Translations,
                RawTranslations = screenSet.RawTranslations,
                CompressionType = screenSet.CompressionType
            }, GlobalUsings.JsonSerializerOptions);
    }

    public override Task<string> Serialize()
        => Task.FromResult(JsonSerializer.Serialize(new ScreenSetsConfigOnDiskModel
        {
            InheritFrom = InheritFrom,
            ScreenSets = Resource.ScreenSets.Select(x => x.ScreenSetId).ToList()
        }, GlobalUsings.JsonSerializerOptions));

    internal class ScreenSetsConfigOnDiskModel
    {
        public string? InheritFrom { get; set; }
        public List<string> ScreenSets { get; set; }
    }

    internal class ScreenSetOnDiskModel
    {
        public string ScreenSetId { get; set; }
        public Dictionary<string, Dictionary<string, string>> Translations { get; set; }
        public string RawTranslations { get; set; }
        public long CompressionType { get; set; }
    }
}
