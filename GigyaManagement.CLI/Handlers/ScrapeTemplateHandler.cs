using GigyaManagement.CLI.Services.Context;
using GigyaManagement.CLI.Services.GigyaApi;
using GigyaManagement.CLI.Services.GigyaApi.Models;
using GigyaManagement.CLI.Services.Project.ProjectModels;
using GigyaManagement.CLI.Services.Template;
using GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

using Mediator;

namespace GigyaManagement.CLI.Handlers;

public class ScrapeTemlateRequest : IRequest<ScrapeTemplateResult>
{
    public string ApiKey { get; set; }

    public bool IsTemplate { get; set; } = false;

    public string Environment { get; set; }

    public string SolutionName { get; set; }
}

public class ScrapeTemplateResult
{
    public string ProjectPath { get; internal set; }
}

public class ScrapeTemplateHandler : IRequestHandler<ScrapeTemlateRequest, ScrapeTemplateResult>
{
    private readonly IGigyaResourceConfigurator<SiteConfig, string> _siteConfigConfigurator;
    private readonly IGigyaResourceConfigurator<AccountsSchema, string> _accountsSchemaConfigurator;
    private readonly IGigyaResourceConfigurator<ScreenSetsConfig, string> _screenSetsConfigurator;
    private readonly IProjectManager _projectManager;
    private readonly IContextService _contextService;

    public ScrapeTemplateHandler(
        IGigyaResourceConfigurator<SiteConfig, string> siteConfigConfigurator,
        IGigyaResourceConfigurator<AccountsSchema, string> accountsSchemaConfigurator,
        IGigyaResourceConfigurator<ScreenSetsConfig, string> screenSetsConfigurator,
        IProjectManager projectManager,
        IContextService contextService)
    {
        _siteConfigConfigurator = siteConfigConfigurator;
        _accountsSchemaConfigurator = accountsSchemaConfigurator;
        _screenSetsConfigurator = screenSetsConfigurator;
        _projectManager = projectManager;
        _contextService = contextService;
    }

    public async ValueTask<ScrapeTemplateResult> Handle(ScrapeTemlateRequest request, CancellationToken cancellationToken)
    {
        var gigProject = new TemplateProject
        {
            SiteConfigResource = new SiteConfigResource { Resource = await _siteConfigConfigurator.Extract(request.ApiKey) },
            AccountsSchemaResource = new AccountsSchemaResource { Resource = await _accountsSchemaConfigurator.Extract(request.ApiKey) },
            ScreenSetsResource = new ScreenSetsResource { Resource = await _screenSetsConfigurator.Extract(request.ApiKey) },
            DataFlowsResource = new(),
            EmailTemplatesResource = new(),
            ExtensionsResource = new(),
            FlowsResource = new(),
            HostedPagesResource = new(),
            IdentitySecurityResource = new(),
            SmsTemplatesResource = new()
        };

        if (_contextService.GetCurrentContext() == null)
        {
            throw new Exception("Context not set");
        }

        var templatesFolder = Path.Combine(_contextService.GetCurrentContext().Workspace, "_template");

        Directory.CreateDirectory(templatesFolder);
            
        await gigProject.PersistToDisk(templatesFolder); // ensure folder structure is created
      
        return new ScrapeTemplateResult();

    }
}
