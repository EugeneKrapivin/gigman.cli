using GigyaManagement.Core;

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

public interface IPersist
{
    public Task<string> Serialize();

    public Task<string> PersistToDisk(string projectPath);
}
public interface IPersistable : IPersist
{
    [JsonIgnore]
    public static abstract string ConfigFileName { get; }
}

public interface ILoadable<T> where T: ILoadable<T>, IPersistable
{
    public abstract static Task<T> Load(string path);
}

public static class ProjectResource
{
    public static Task<T?> Load<T>(string path)
          where T : IPersistable
    {
        var file = path;
        var conf = T.ConfigFileName;

        if (!path.EndsWith(conf))
        {
            var files = Directory.GetFiles(path);

            var exists = files.Select(x => Path.GetFileName(x)).Any(x => x == conf);

            if (!exists)
            {
                throw new Exception($"load path \"{path}\" doesn't contain a valid \"{conf}\" file.");
            }

            file = Path.Combine(path, conf);
        }

        var project = JsonSerializer.Deserialize<T>(File.ReadAllText(file), GlobalUsings.JsonSerializerOptions);

        return Task.FromResult(project);
    }
}

public abstract class ProjectResource<T>
{
    public T Resource { get; init; }

    public string? InheritFrom { get; set; }

    public virtual Task<string> Serialize() => Task.FromResult(JsonSerializer.Serialize(this, GlobalUsings.JsonSerializerOptions));

    public virtual async Task<string> PersistToDisk(string projectPath, string configFileName)
    {
        var content = await Serialize();
        var path = Path.Combine(projectPath, configFileName);

        File.WriteAllText(path, content);

        return path;
    }
}
