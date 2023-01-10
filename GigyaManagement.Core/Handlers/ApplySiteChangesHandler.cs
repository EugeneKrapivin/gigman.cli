using GigyaManagement.CLI.Services.Context;
using GigyaManagement.CLI.Services.GigyaApi;
using GigyaManagement.CLI.Services.GigyaApi.Models;
using GigyaManagement.CLI.Services.Project.ProjectModels;

using Mediator;
namespace GigyaManagement.CLI.Handlers;


public class ApplySiteChangesRequest : IRequest<ApplySiteChangesResult>
{
    public required string Solution { get; set; }

    public required string Environment { get; init; }

    public string? TargetApiKey { get; set; } = null;
    
    public bool SelfApply { get; set; } = true;
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

    public ApplySiteChangesHandler(
        IGigyaResourceConfigurator<SiteConfig, string> siteConfigConfigurator,
        IGigyaResourceConfigurator<AccountsSchema, string> accountsSchemaConfigurator,
        IGigyaResourceConfigurator<ScreenSetsConfig, string> screenSetsConfigurator,
        WorkspaceContext context)
    {
        _siteConfigConfigurator = siteConfigConfigurator;
        _accountsSchemaConfigurator = accountsSchemaConfigurator;
        _screenSetsConfigurator = screenSetsConfigurator;
    }

    public async ValueTask<ApplySiteChangesResult> Handle(ApplySiteChangesRequest request, CancellationToken cancellationToken)
    {
        //var siteFolder = Path.Combine(_context.Workspace, "_sites", request.Solution);

        //// dirty trick - if solution contains a valid path to a solution folder - use that folder as root for operations for apply
        //if (File.Exists(Path.Combine(request.Solution, "site.solution.json")))
        //{
        //    siteFolder = request.Solution;
        //}

        //var solution = await GigyaSolution.Load(siteFolder);

        //var project = solution.SiteProjects.SingleOrDefault(x => x.name == request.Environment);
        
        //if (project == null)
        //{
        //    throw new Exception($"Environment \"{request.Environment}\" was not found for solution \"{request.Solution}\"");
        //}

        //var target = request.SelfApply ? project.Apikey : request.TargetApiKey;

        //if (!request.SelfApply && string.IsNullOrEmpty(request.TargetApiKey))
        //{
        //    throw new Exception("when not applying to self, a target apikey must be passed");
        //}

        //if (project.SiteConfigResource?.Resource is not null) await _siteConfigConfigurator.Apply(target!, project.SiteConfigResource.Resource);
        //if (project.AccountsSchemaResource?.Resource is not null) await _accountsSchemaConfigurator.Apply(target!, project.AccountsSchemaResource.Resource);
        ////if (project.ScreenSetsResource?.Resource is not null) await _screenSetsConfigurator.Apply(target!, project.ScreenSetsResource.Resource);

        return new();
    }
}
