using GigyaManagement.CLI.Services.Context;
using GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

namespace GigyaManagement.CLI.Services.Template;

public interface IProjectManager
{
    public Task<string> SaveProject(SiteProject siteProject);

    public Task<SiteProject> LoadProject(string projectPath);
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

    public Task<SiteProject> LoadProject(string projectPath)
    {
        return SiteProject.Load(projectPath);
    }

    public async Task<string> SaveProject(SiteProject siteProject)
    {
        var path = _partnerContext.Workspace;
        var folder = siteProject.IsTemplate 
            ? _defaultTemplatesFolder 
            : _defaultSitesFolder;
        
        var projectFolder = Path.Combine(path, folder, siteProject.SiteConfigResource.Resource.BaseDomain);
        Directory.CreateDirectory(projectFolder);
        await Console.Out.WriteLineAsync($"writing project resource to \"{projectFolder}\"");

        await siteProject.PersistToDisk(projectFolder);
        
        return projectFolder;
    }
}
