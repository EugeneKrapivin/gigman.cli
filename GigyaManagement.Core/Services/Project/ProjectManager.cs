using GigyaManagement.CLI.Services.Context;
using GigyaManagement.CLI.Services.Project.ProjectModels;

namespace GigyaManagement.CLI.Services.Template;


public class ProjectManagerOptions
{
    public string WorkspacePath { get; set; } = Directory.GetCurrentDirectory();
}

public class ProjectManager
{
    private readonly ProjectManagerOptions _projectManagerOptions;
    private readonly WorkspaceContext _partnerContext;

    public ProjectManager(ProjectManagerOptions projectManagerOptions, WorkspaceContext partnerContext)
    {
        _projectManagerOptions = projectManagerOptions;
        _partnerContext = partnerContext;
    }
}
