using System.Text.Json;
using System.Text.Json.Serialization;

namespace GigyaManagement.CLI.Services.Project.ProjectModels;

public sealed class GigyaSolution
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name { get; set; }

    public int Version { get; } = 1;

    public bool IsTemplate { get; set; } = false;

    public string? InheritFrom { get; set; }

    [JsonIgnore]
    public List<SiteProject> Environments { get; set; } = new();

    [JsonIgnore]
    public static string ConfigFileName => "site.solution.json";

    public required string SolutionFolder { get; init; }

    private GigyaSolution()
    {

    }

    public static async Task<GigyaSolution> Load(string path)
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
        var solution = JsonSerializer.Deserialize<OnDiskModel>(fileContent, GlobalUsings.JsonSerializerOptions);

        if (solution is null)
        {
            throw new Exception($"failed to parse the solution file \"{fileContent}\"");
        }

        var sol = new GigyaSolution
        {
            Name = solution.Name,
            Id = solution.Id,
            InheritFrom = solution.InheritFrom,
            IsTemplate = solution.IsTemplate,
            SolutionFolder = path
        };

        foreach (var env in solution.Environments)
        {
            var project = await SiteProject.Load(env.Value);
            sol.Environments.Add(project);
        }

        return sol;
    }

    public static GigyaSolution New(string solutionName, string solutionPath)
    {
        return new GigyaSolution
        {
            Name = solutionName,
            SolutionFolder = solutionPath
        };
    }

    public async Task<string> PersistToDisk()
    {
        var projects = new List<(string env, string path)>();
        foreach (var site in this.Environments)
        {
            var projectConfPath = await site.PersistToDisk(SolutionFolder);
            projects.Add(projectConfPath);
        }

        var content = JsonSerializer.Serialize(new OnDiskModel
        {
            Id = Id,
            Name = Name,
            InheritFrom = InheritFrom,
            IsTemplate = IsTemplate,
            Environments = projects.ToDictionary(x => x.env, x => x.path)
        }, GlobalUsings.JsonSerializerOptions);

        var path = Path.Combine(SolutionFolder, ConfigFileName);
        Directory.CreateDirectory(SolutionFolder);

        File.WriteAllText(path, content);

        return path;
    }

    private class OnDiskModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Version { get; } = 1;

        public bool IsTemplate { get; set; } = false;

        public string? InheritFrom { get; set; }

        public Dictionary<string, string> Environments { get; set; } = new();
    }

    public void Add(SiteProject resource)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));

        if (!Environments.Any(x => x.Apikey == resource.Apikey))
        {
            Environments.Add(resource);
        }
    }
}
