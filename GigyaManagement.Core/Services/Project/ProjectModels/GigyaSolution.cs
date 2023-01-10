using GigyaManagement.Core;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace GigyaManagement.CLI.Services.Project.ProjectModels;

public sealed class GigyaSolution
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name { get; set; }

    public int Version { get; } = 1;

    public bool IsTemplate { get; set; } = false;

    public string? InheritFrom { get; set; }

    public List<SiteProject> SiteProjects { get; internal set; } = new List<SiteProject>();
    [JsonIgnore]
    public static string ConfigFileName => "site.solution.json";

    public required string SolutionFolder { get; init; }

    private GigyaSolution()
    {

    }

    public static async Task<GigyaSolution> Load(string path)
    {
        var solutionFile = FindSolutionFile(path);

        var solutionOnDiskModel = await ParseSolutionFile(solutionFile);

        var solution = new GigyaSolution
        {
            Name = solutionOnDiskModel.Name,
            Id = solutionOnDiskModel.Id,
            InheritFrom = solutionOnDiskModel.InheritFrom,
            IsTemplate = solutionOnDiskModel.IsTemplate,
            SolutionFolder = path
        };

        foreach (var site in solutionOnDiskModel.Sites)
        {
            var project = await SiteProject.Load(site.ToString());
            solution.SiteProjects.Add(project);
        }

        return solution;
    }

    private static async Task<OnDiskModel> ParseSolutionFile(string solutionFile)
    {
        var fileContent = await File.ReadAllTextAsync(solutionFile);
        var solution = JsonSerializer.Deserialize<OnDiskModel>(fileContent, GlobalUsings.JsonSerializerOptions)
            ?? throw new Exception($"failed to parse the solutionOnDiskModel solutionFile \"{fileContent}\"");
        
        return solution;
    }

    private static string FindSolutionFile(string path)
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

        return file;
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
        var sitePersistTasks = SiteProjects.Select(x => x.PersistToDisk(SolutionFolder)).ToList();

        await Task.WhenAll(sitePersistTasks);

        var content = JsonSerializer.Serialize(new OnDiskModel
        {
            Id = Id,
            Name = Name,
            InheritFrom = InheritFrom,
            IsTemplate = IsTemplate,
            Sites = sitePersistTasks.Select(x => new Uri(Path.GetRelativePath(SolutionFolder, x.Result), UriKind.Relative)).ToList()
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

        public IEnumerable<Uri> Sites { get; set; }
    }

    public void Add(SiteProject resource)
    {
        if (resource == null) throw new ArgumentNullException(nameof(resource));

        // ensure we are not adding a site project already added to this solution
        var parent = SiteProjects.SingleOrDefault(x => x.Apikey == resource.Apikey);
        if (parent == null)
        {
            SiteProjects.Add(resource);
        }
    }
}
