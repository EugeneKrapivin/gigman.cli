using GigyaManagement.CLI.Services.Context;
using GigyaManagement.CLI.Services.Project.ProjectModels;

namespace GigyaManagement.CLI.Services.Template;

public interface IProjectManager
{
    public Task<GigyaSolution> LoadSolution(string solutionPath);
}

public class ProjectManagerOptions
{
    public string WorkspacePath { get; set; } = Directory.GetCurrentDirectory();
}

public class ProjectManager : IProjectManager
{
    const string _defaultTemplatesFolder = "_templates/";
    const string _defaultSitesFolder = "_sites/";

    private readonly ProjectManagerOptions _projectManagerOptions;
    private readonly WorkspaceContext _partnerContext;

    public ProjectManager(ProjectManagerOptions projectManagerOptions, WorkspaceContext partnerContext)
    {
        _projectManagerOptions = projectManagerOptions;
        _partnerContext = partnerContext;
    }

    public Task<GigyaSolution> LoadSolution(string solutionPath)
    {
        return GigyaSolution.Load(solutionPath);
    }
}
