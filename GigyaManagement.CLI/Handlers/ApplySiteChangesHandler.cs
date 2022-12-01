using GigyaManagement.CLI.Services.Context;
using GigyaManagement.CLI.Services.GigyaApi;
using GigyaManagement.CLI.Services.GigyaApi.Models;
using GigyaManagement.CLI.Services.Template;

using Mediator;
namespace GigyaManagement.CLI.Handlers;


public class ApplySiteChangesRequest : IRequest<ApplySiteChangesResult>
{
    public required string Site { get; init; }
    public bool IsTemplate { get; set; } = false;
}

public class ApplySiteChangesResult
{

}

public class ApplySiteChangesHandler : IRequestHandler<ApplySiteChangesRequest, ApplySiteChangesResult>
{
    private readonly IGigyaResourceConfigurator<SiteConfig, string> _siteConfigConfigurator;
    private readonly IGigyaResourceConfigurator<AccountsSchema, string> _accountsSchemaConfigurator;
    private readonly IGigyaResourceConfigurator<ScreenSetsConfig, string> _screenSetsConfigurator;
    private readonly WorkspaceContext _context;
    private readonly IProjectManager _projectManager;

    public ApplySiteChangesHandler(
        IGigyaResourceConfigurator<SiteConfig, string> siteConfigConfigurator,
        IGigyaResourceConfigurator<AccountsSchema, string> accountsSchemaConfigurator,
        IGigyaResourceConfigurator<ScreenSetsConfig, string> screenSetsConfigurator,
        WorkspaceContext context,
        IProjectManager projectManager)
    {
        _siteConfigConfigurator = siteConfigConfigurator;
        _accountsSchemaConfigurator = accountsSchemaConfigurator;
        _screenSetsConfigurator = screenSetsConfigurator;
        _context = context;
        _projectManager = projectManager;
    }

    public async ValueTask<ApplySiteChangesResult> Handle(ApplySiteChangesRequest request, CancellationToken cancellationToken)
    {
        var siteFolder = Path.Combine(_context.Workspace, ProjectFolderScope(request.IsTemplate), request.Site);

        var project = await _projectManager.LoadProject(siteFolder);

        if (project.SiteConfigResource?.Resource is not null) await _siteConfigConfigurator.Apply(project.ApiKey, project.SiteConfigResource.Resource);
        if (project.AccountsSchemaResource?.Resource is not null) await _accountsSchemaConfigurator.Apply(project.ApiKey, project.AccountsSchemaResource.Resource);
        if (project.ScreenSetsResource?.Resource is not null) await _screenSetsConfigurator.Apply(project.ApiKey, project.ScreenSetsResource.Resource);

        return new();

        static string ProjectFolderScope(bool isTemplate)
            => isTemplate ? "_templates" : "_sites";
    }
}
