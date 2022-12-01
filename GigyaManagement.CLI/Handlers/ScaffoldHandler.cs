using GigyaManagement.CLI.Services.GigyaApi;
using GigyaManagement.CLI.Services.GigyaApi.Models;
using GigyaManagement.CLI.Services.Template;
using GigyaManagement.CLI.Services.Template.ProjectModels;
using GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

using Mediator;

namespace GigyaManagement.CLI.Handlers;

public class ScrapeSiteRequest : IRequest<ScrapeSiteResult>
{
    public string ApiKey { get; set; }
    public bool IsTemplate { get; set; } = false;
}

public class ScrapeSiteResult
{

}

public class ScaffoldHandler : IRequestHandler<ScrapeSiteRequest, ScrapeSiteResult>
{
    private readonly IGigyaResourceConfigurator<SiteConfig, string> _siteConfigConfigurator;
    private readonly IGigyaResourceConfigurator<AccountsSchema, string> _accountsSchemaConfigurator;
    private readonly IGigyaResourceConfigurator<ScreenSetsConfig, string> _screenSetsConfigurator;
    private readonly IProjectManager _projectManager;

    public ScaffoldHandler(
        IGigyaResourceConfigurator<SiteConfig, string> siteConfigConfigurator,
        IGigyaResourceConfigurator<AccountsSchema, string> accountsSchemaConfigurator,
        IGigyaResourceConfigurator<ScreenSetsConfig, string> screenSetsConfigurator,
        IProjectManager projectManager)
    {
        _siteConfigConfigurator = siteConfigConfigurator;
        _accountsSchemaConfigurator = accountsSchemaConfigurator;
        _screenSetsConfigurator = screenSetsConfigurator;
        _projectManager = projectManager;
    }
    
    public async ValueTask<ScrapeSiteResult> Handle(ScrapeSiteRequest request, CancellationToken cancellationToken)
    {
        var gigProject = new SiteProject
        {
            ApiKey = request.ApiKey,
            IsTemplate = request.IsTemplate,
            SiteConfigResource = new SiteConfigResource { Resource = await _siteConfigConfigurator.Extract(request.ApiKey) },
            AccountsSchemaResource = new AccountsSchemaResource { Resource = await _accountsSchemaConfigurator.Extract(request.ApiKey) },
            ScreenSetsResource = new ScreenSetsResource { Resource = await _screenSetsConfigurator.Extract(request.ApiKey)},
            DataFlowsResource = new(),
            EmailTemplatesResource = new(),
            ExtensionsResource = new(),
            FlowsResource = new(),
            HostedPagesResource = new(),
            IdentitySecurityResource = new(),
            SmsTemplatesResource = new()
        };
        
        var path = await _projectManager.SaveProject(gigProject);

        //var project = await _projectManager.LoadProject(path);

        return new ScrapeSiteResult();
    }
}
